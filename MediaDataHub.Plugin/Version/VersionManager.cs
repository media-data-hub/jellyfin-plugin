using Microsoft.Extensions.Logging;
using System.Globalization;
using Jellyfin.Data.Enums;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Entities;

namespace MediaDataHub.Plugin.Version;

public class VersionManager
{
  private readonly ILogger<VersionManager> _logger;
  private readonly ILibraryManager _libraryManager;

  public VersionManager(
    ILibraryManager libraryManager,
    ILogger<VersionManager> logger
  )
  {
    _libraryManager = libraryManager;
    _logger = logger;
  }
  public void MergeEpisodes(IProgress<double> progress)
  {
    var episodes = GetEpisodesFromLibrary().ToArray();

    _logger.LogInformation("Scanning for repeated episodes");

    //Group by the Series name, Season name , episode name, episode number and year, then select those with more than 1 in the group
    var duplications = episodes.GroupBy(x => x.GetProviderId(Plugin.ProviderId)).Where(x => x.Count() > 1 && x.Key != null).ToList();

    var total = duplications.Count;
    var current = 0;
    //foreach grouping, merge
    foreach (var e in duplications)
    {
      current++;
      var percent = current / (double)total * 100;
      progress?.Report((int)percent);
      MergeVideos(e.ToList().Where(e => e.PrimaryVersionId == null && !e.GetLinkedAlternateVersions().Any()));
    }
    progress?.Report(100);
  }

  public void SplitEpisodes(IProgress<double> progress)
  {
    var episodes = GetEpisodesFromLibrary().ToArray();
    var total = episodes.Length;
    var current = 0;
    //foreach grouping, merge
    foreach (var e in episodes)
    {
      current++;
      var percent = ((double)current / (double)total) * 100;
      progress?.Report((int)percent);

      _logger.LogInformation($"Spliting {e.IndexNumber} ({e.SeriesName})");
      SplitVideo(e);
    }
    progress?.Report(100);

  }

  public void MergeMovies(IProgress<double> progress)
  {
    var movies = GetMoviesFromLibrary().ToArray();
    _logger.LogInformation("Scanning for repeated movies");
    //Group by Id, then select those with more than 1 in the group
    var duplications = movies.GroupBy(x => x.GetProviderId(Plugin.ProviderId)).Where(x => x.Count() > 1 && x.Key != null).ToList();
    var total = duplications.Count;
    var current = 0;
    //foreach grouping, merge
    Parallel.ForEach(duplications, m =>
    {
      current++;
      var percent = current / (double)total * 100;
      progress?.Report((int)percent);
      MergeVideos(m.Where(m => m.PrimaryVersionId == null && !m.GetLinkedAlternateVersions().Any()).ToList()); //We only want non merged movies
    });
    progress?.Report(100);
  }

  public void SplitMovies(IProgress<double> progress)
  {
    var movies = GetMoviesFromLibrary().ToArray();
    var total = movies.Length;
    var current = 0;
    //foreach grouping, merge
    Parallel.ForEach(movies, m =>
     {
       current++;
       var percent = ((double)current / (double)total) * 100;
       progress?.Report((int)percent);

       _logger.LogInformation($"Spliting {m.Name} ({m.ProductionYear})");
       SplitVideo(m);
     }
    );
    progress?.Report(100);
  }

  private static async void SplitVideo(Video video)
  {
    if (video is null)
    {
      return;
    }

    foreach (var link in video.GetLinkedAlternateVersions())
    {
      link.SetPrimaryVersionId(null);
      link.LinkedAlternateVersions = [];

      await link.UpdateToRepositoryAsync(ItemUpdateType.MetadataEdit, CancellationToken.None).ConfigureAwait(false);
    }

    video.LinkedAlternateVersions = [];
    video.SetPrimaryVersionId(null);
    await video.UpdateToRepositoryAsync(ItemUpdateType.MetadataEdit, CancellationToken.None).ConfigureAwait(false);
  }

  private static List<Video> FlatVideos(Video video)
  {
    var videos = new List<Video> { video };
    video.SetPrimaryVersionId(null);
    videos.AddRange(video.GetLinkedAlternateVersions().SelectMany(FlatVideos));
    video.LinkedAlternateVersions = [];
    return videos;
  }

  // https://github.com/jellyfin/jellyfin/blob/master/Jellyfin.Api/Controllers/VideosController.cs
  private static async void MergeVideos(IEnumerable<Video> videos)
  {
    var items = videos.SelectMany(FlatVideos)
    .OrderBy(i => i.Video3DFormat.HasValue || i.VideoType != VideoType.VideoFile ? 1 : 0)
    .ThenByDescending(i => i.GetDefaultVideoStream()?.Width ?? 0)
    .ThenBy(i => i.GetMediaSources(false).First().Name)
    .ThenByDescending(i => i.GetMediaStreams().Where(m => m.Type == MediaStreamType.Audio).Count())
    .ThenByDescending(i => i.GetMediaStreams().Where(m => m.Type == MediaStreamType.Subtitle).Count());

    if (items.Count() < 2)
    {
      return;
    }

    var primaryVersion = items.First();
    var alternateVersionsOfPrimary = primaryVersion.LinkedAlternateVersions.ToList();

    foreach (var item in items.Where(i => !i.Id.Equals(primaryVersion.Id)))
    {
      item.SetPrimaryVersionId(primaryVersion.Id.ToString("N", CultureInfo.InvariantCulture));

      await item.UpdateToRepositoryAsync(ItemUpdateType.MetadataEdit, CancellationToken.None).ConfigureAwait(false);

      if (!alternateVersionsOfPrimary.Any(i => string.Equals(i.Path, item.Path, StringComparison.OrdinalIgnoreCase)))
      {
        alternateVersionsOfPrimary.Add(new LinkedChild
        {
          Path = item.Path,
          ItemId = item.Id
        });
      }

      foreach (var linkedItem in item.LinkedAlternateVersions)
      {
        if (!alternateVersionsOfPrimary.Any(i => string.Equals(i.Path, linkedItem.Path, StringComparison.OrdinalIgnoreCase)))
        {
          alternateVersionsOfPrimary.Add(linkedItem);
        }
      }

      if (item.LinkedAlternateVersions.Length > 0)
      {
        item.LinkedAlternateVersions = [];
        await item.UpdateToRepositoryAsync(ItemUpdateType.MetadataEdit, CancellationToken.None).ConfigureAwait(false);
      }
    }

    primaryVersion.LinkedAlternateVersions = alternateVersionsOfPrimary.ToArray();
    await primaryVersion.UpdateToRepositoryAsync(ItemUpdateType.MetadataEdit, CancellationToken.None).ConfigureAwait(false);
  }

  private IEnumerable<Episode> GetEpisodesFromLibrary()
  {
    var query = new InternalItemsQuery
    {
      IncludeItemTypes = [BaseItemKind.Episode],
      IsVirtualItem = false,
      Recursive = true
    };
    return _libraryManager.GetItemList(query)
      .Select(item => item as Episode)
      .Where(item => item != null)
      .OfType<Episode>()
      .ToList();
  }

  private IEnumerable<Movie> GetMoviesFromLibrary()
  {
    var query = new InternalItemsQuery
    {
      IncludeItemTypes = [BaseItemKind.Movie],
      IsVirtualItem = false,
      Recursive = true
    };
    return _libraryManager.GetItemList(query)
      .Select(item => item as Movie)
      .Where(item => item != null)
      .OfType<Movie>()
      .ToList();
  }
}

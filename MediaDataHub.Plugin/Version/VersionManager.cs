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
    Parallel.ForEach(duplications, v =>
    {
      current++;
      var percent = (double)current / total * 100;
      progress?.Report((int)percent);
      MergeVideos(v.Where(m => m.PrimaryVersionId == null && !m.GetLinkedAlternateVersions().Any()).ToList());
    });
    progress?.Report(100);
  }

  public void SplitEpisodes(IProgress<double> progress)
  {
    var episodes = GetEpisodesFromLibrary().ToArray();
    var total = episodes.Length;
    var current = 0;
    //foreach grouping, merge
    foreach (var episode in episodes)
    {
      current++;
      var percent = (double)current / total * 100;
      progress?.Report((int)percent);

      if (episode is null)
      {
        continue;
      }
      if (episode.PrimaryVersionId is null && !episode.GetLinkedAlternateVersions().Any())
      {
        continue;
      }

      _logger.LogInformation("Splitting {IndexNumber} ({SeriesName})", episode.IndexNumber, episode.SeriesName);
      SplitVideo(episode);
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
    Parallel.ForEach(duplications, v =>
    {
      current++;
      var percent = (double)current / total * 100;
      progress?.Report((int)percent);
      MergeVideos(v.Where(m => m.PrimaryVersionId == null && !m.GetLinkedAlternateVersions().Any()).ToList());
    });
    progress?.Report(100);
  }

  public void SplitMovies(IProgress<double> progress)
  {
    var movies = GetMoviesFromLibrary().ToArray();
    var total = movies.Length;
    var current = 0;
    //foreach grouping, merge
    foreach (var movie in movies)
    {
      current++;
      var percent = (double)current / total * 100;
      progress?.Report((int)percent);

      if (movie is null)
      {
        continue;
      }
      if (movie.PrimaryVersionId is null && !movie.GetLinkedAlternateVersions().Any())
      {
        continue;
      }

      _logger.LogInformation("Splitting {Name} ({ProductionYear})", movie.Name, movie.ProductionYear);
      SplitVideo(movie);
    }
    progress?.Report(100);
  }

  private static async void SplitVideo(Video video)
  {
    foreach (var link in video.GetLinkedAlternateVersions())
    {
      link.SetPrimaryVersionId(null);
      link.LinkedAlternateVersions = [];
      await link.UpdateToRepositoryAsync(ItemUpdateType.MetadataEdit, CancellationToken.None).ConfigureAwait(false);
    }

    video.SetPrimaryVersionId(null);
    video.LinkedAlternateVersions = [];
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
  private async void MergeVideos(IEnumerable<Video> videos)
  {
    var items = videos.SelectMany(FlatVideos)
    .DistinctBy(v => v.Id)
    .OrderBy(i => i.Video3DFormat.HasValue || i.VideoType != VideoType.VideoFile ? 1 : 0)
    .ThenByDescending(i => i.GetDefaultVideoStream()?.Width ?? 0)
    .ThenBy(i => Path.GetFileNameWithoutExtension(i.Path))
    .ThenByDescending(i => i.GetMediaStreams().Where(m => m.Type == MediaStreamType.Audio).Count())
    .ThenByDescending(i => i.GetMediaStreams().Where(m => m.Type == MediaStreamType.Subtitle).Count());

    if (items.Count() < 2)
    {
      return;
    }

    var primaryVersion = items.First();
    var alternateVersionsOfPrimary = primaryVersion.LinkedAlternateVersions.ToList();

    _logger.LogInformation("Merging {Name} ({id})", Path.GetFileNameWithoutExtension(primaryVersion.Path) ?? "", primaryVersion.Id);

    foreach (var item in items.Where(i => !i.Id.Equals(primaryVersion.Id)))
    {
      _logger.LogInformation("Merging {Name} ({id}) to {Name} ({id})", Path.GetFileNameWithoutExtension(item.Path), item.Id, Path.GetFileNameWithoutExtension(primaryVersion.Path) ?? "", primaryVersion.Id);
      item.SetPrimaryVersionId(primaryVersion.Id.ToString("N", CultureInfo.InvariantCulture));
      item.LinkedAlternateVersions = [];
      await item.UpdateToRepositoryAsync(ItemUpdateType.MetadataEdit, CancellationToken.None).ConfigureAwait(false);

      if (!alternateVersionsOfPrimary.Any(i => string.Equals(i.Path, item.Path, StringComparison.OrdinalIgnoreCase)))
      {
        alternateVersionsOfPrimary.Add(new LinkedChild { Path = item.Path, ItemId = item.Id });
      }
    }

    primaryVersion.SetPrimaryVersionId(null);
    primaryVersion.LinkedAlternateVersions = [.. alternateVersionsOfPrimary];
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

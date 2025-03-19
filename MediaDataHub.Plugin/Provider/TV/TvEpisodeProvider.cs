using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using MediaDataHub.Plugin.Api;
using MediaDataHub.Plugin.Api.Manager;
using Model = MediaDataHub.Plugin.Api.Model;
using Jellyfin.Extensions;

namespace MediaDataHub.Plugin.Provider.TV;

public class TvEpisodeProvider : MediaDataHubBaseProvider<Model.TvEpisode, Episode, EpisodeInfo>
{
  private readonly MediaDataHubApiManager _apiManager;

  public TvEpisodeProvider(
    ILogger<TvEpisodeProvider> logger,
    MediaDataHubApiManager apiManager,
    IHttpClientFactory httpClientFactory
  ) : base(logger, httpClientFactory)
  {
    _apiManager = apiManager;
  }

  /// <inheritdoc />
  public override Task<IEnumerable<RemoteSearchResult>> GetSearchResults(EpisodeInfo info, CancellationToken cancellationToken)
  {
    _logger.LogInformation("GetSearchResults Episode");
    return Task.FromResult(Enumerable.Empty<RemoteSearchResult>());
  }

  /// <inheritdoc />
  public override async Task<MetadataResult<Episode>> GetMetadata(EpisodeInfo info, CancellationToken cancellationToken)
  {
    if (info.IsMissingEpisode)
    {
      return new MetadataResult<Episode>();
    }

    info.SeriesProviderIds.TryGetValue(Plugin.ProviderId, out string? seriesId);
    var seasonNumber = info.ParentIndexNumber;
    var episodeNumber = info.IndexNumber;

    if (string.IsNullOrWhiteSpace(seriesId) || !seasonNumber.HasValue || !episodeNumber.HasValue)
    {
      _logger.LogInformation("GetMetadata ({seriesId}, {seasonNumber}, {episodeNumber})", seriesId, seasonNumber.GetValueOrDefault(), episodeNumber.GetValueOrDefault());
      return new MetadataResult<Episode>();
    }

    try
    {
      if (info.IndexNumberEnd.HasValue)
      {
        var episodeNumberEnd = info.IndexNumberEnd;
        IEnumerable<IMetadataResult<Episode, EpisodeInfo>> details = await _apiManager
        .SearchTvEpisodes(
          seriesId,
          seasonNumber.GetValueOrDefault(),
          episodeNumber.GetValueOrDefault(),
          episodeNumberEnd.GetValueOrDefault(),
          cancellationToken
        )
        .ConfigureAwait(false);
        var results = details.Select(detail => detail.ToMetadataResult(info)).ToList();
        if (results.Count <= 0)
        {
          _logger.LogInformation("GetMetadata (No detail)");
          return new MetadataResult<Episode>();
        }
        var result = results.First();
        if (results.Count == 1)
        {
          return result;
        }
        results.RemoveAt(0);
        foreach (var detail in results)
        {
          if (!string.IsNullOrWhiteSpace(detail.Item.Name))
          {
            if (string.IsNullOrWhiteSpace(result.Item.Name))
            {
              result.Item.Name = detail.Item.Name;
            }
            else
            {
              result.Item.Name += $" / {detail.Item.Name}";
            }
          }

          if (!string.IsNullOrWhiteSpace(detail.Item.OriginalTitle))
          {
            if (string.IsNullOrWhiteSpace(result.Item.OriginalTitle))
            {
              result.Item.OriginalTitle = detail.Item.OriginalTitle;
            }
            else
            {
              result.Item.OriginalTitle += $" / {detail.Item.OriginalTitle}";
            }
          }

          if (!string.IsNullOrWhiteSpace(detail.Item.Overview))
          {
            if (string.IsNullOrWhiteSpace(result.Item.Overview))
            {
              result.Item.Overview = detail.Item.Overview;
            }
            else
            {
              result.Item.Overview += $"<br/>\n<br/>\n<br/>\n{detail.Item.Overview}";
            }
          }

          if (!string.IsNullOrWhiteSpace(detail.Item.Tagline))
          {
            if (string.IsNullOrWhiteSpace(result.Item.Tagline))
            {
              result.Item.Tagline = detail.Item.Tagline;
            }
            else
            {
              result.Item.Tagline += $" / {detail.Item.Tagline}";
            }
          }
          foreach (var tag in detail.Item.Tags)
          {
            if (!result.Item.Tags.Contains(tag))
            {
              result.Item.Tags = [.. result.Item.Tags, tag];
            }
          }
          foreach (var genre in detail.Item.Genres)
          {
            if (!result.Item.Genres.Contains(genre))
            {
              result.Item.Genres = [.. result.Item.Genres, genre];
            }
          }
          foreach (var studio in detail.Item.Studios)
          {
            if (!result.Item.Studios.Contains(studio))
            {
              result.Item.Studios = [.. result.Item.Studios, studio];
            }
          }
          foreach (var location in detail.Item.ProductionLocations)
          {
            if (!result.Item.ProductionLocations.Contains(location))
            {
              result.Item.ProductionLocations = [.. result.Item.ProductionLocations, location];
            }
          }
          result.Item.EndDate = detail.Item.EndDate;
          foreach (var person in detail.People)
          {
            result.AddPerson(person);
          }
        }
        result.Item.IndexNumberEnd = episodeNumberEnd;
        return result;
      }
      else
      {
        IMetadataResult<Episode, EpisodeInfo>? detail = await _apiManager.SearchTvEpisode(seriesId, seasonNumber.GetValueOrDefault(), episodeNumber.GetValueOrDefault(), cancellationToken).ConfigureAwait(false);
        if (detail == null)
        {
          _logger.LogInformation("GetMetadata (No detail)");
          return new MetadataResult<Episode>();
        }
        return detail.ToMetadataResult(info);
      }
    }
    catch (Model.ApiException e)
    {
      _logger.LogWarning(e, "Failed to load ({seriesId}, {EpisodeNumber})", seriesId, episodeNumber);
      return new MetadataResult<Episode>();
    }
  }

  /// <inheritdoc />
  protected override Task<Model.TvEpisode> GetById(string id, CancellationToken cancellationToken)
  {
    return _apiManager.GetTvEpisodeById(id, cancellationToken);
  }

  public override IEnumerable<ImageType> GetSupportedImages(BaseItem item) => [ImageType.Primary, ImageType.Backdrop, ImageType.Banner, ImageType.Logo, ImageType.Thumb];
}

using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using MediaDataHub.Plugin.Api;
using MediaDataHub.Plugin.Api.Manager;
using Model = MediaDataHub.Plugin.Api.Model;

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
      IMetadataResult<Episode, EpisodeInfo>? detail = await _apiManager.SearchTvEpisode(seriesId, seasonNumber.GetValueOrDefault(), episodeNumber.GetValueOrDefault(), cancellationToken).ConfigureAwait(false);
      if (detail == null)
      {
        _logger.LogInformation("GetMetadata (No detail)");
        return new MetadataResult<Episode>();
      }
      return detail.ToMetadataResult(info);
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

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

public class TvSeasonProvider : MediaDataHubBaseProvider<Model.TvSeason, Season, SeasonInfo>
{
  private readonly MediaDataHubApiManager _apiManager;

  public TvSeasonProvider(
    ILogger<TvSeasonProvider> logger,
    MediaDataHubApiManager apiManager,
    IHttpClientFactory httpClientFactory
  ) : base(logger, httpClientFactory)
  {
    _apiManager = apiManager;
  }

  /// <inheritdoc />
  public override Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SeasonInfo info, CancellationToken cancellationToken)
  {
    _logger.LogInformation("GetSearchResults Season");
    return Task.FromResult(Enumerable.Empty<RemoteSearchResult>());
  }

  /// <inheritdoc />
  public override async Task<MetadataResult<Season>> GetMetadata(SeasonInfo info, CancellationToken cancellationToken)
  {
    info.SeriesProviderIds.TryGetValue(Plugin.ProviderId, out string? seriesId);
    var seasonNumber = info.IndexNumber;

    if (string.IsNullOrWhiteSpace(seriesId) || !seasonNumber.HasValue)
    {
      _logger.LogInformation("GetMetadata ({seriesId}, {seasonNumber})", seriesId, seasonNumber.GetValueOrDefault());
      return new MetadataResult<Season>();
    }

    try
    {
      IMetadataResult<Season, SeasonInfo>? detail = await _apiManager.SearchTvSeason(seriesId, seasonNumber.GetValueOrDefault(), cancellationToken).ConfigureAwait(false);
      if (detail == null)
      {
        _logger.LogInformation("GetMetadata (No detail)");
        return new MetadataResult<Season>();
      }
      return detail.ToMetadataResult(info);
    }
    catch (Model.ApiException e)
    {
      _logger.LogWarning(e, "Failed to load ({seriesId}, {seasonNumber})", seriesId, seasonNumber);
      return new MetadataResult<Season>();
    }
  }
  /// <inheritdoc />
  protected override Task<Model.TvSeason> GetById(string id, CancellationToken cancellationToken)
  {
    return _apiManager.GetTvSeasonById(id, cancellationToken);
  }

  public override IEnumerable<ImageType> GetSupportedImages(BaseItem item) => new[] { ImageType.Primary, ImageType.Backdrop, ImageType.Banner, ImageType.Logo, ImageType.Thumb };
}

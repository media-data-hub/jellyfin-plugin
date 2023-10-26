using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaDataHub.Plugin.Api;
using MediaDataHub.Plugin.Api.Manager;
using Model = MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Provider.TV;

public class TvSeriesProvider : MediaDataHubProvider<Model.TvSeries, Series, SeriesInfo>
{
  private readonly MediaDataHubApiManager _apiManager;

  public TvSeriesProvider(
    ILogger<TvSeriesProvider> logger,
    MediaDataHubApiManager apiManager,
    IHttpClientFactory httpClientFactory
  ) : base(logger, httpClientFactory)
  {
    _apiManager = apiManager;
  }

  /// <inheritdoc />
  protected override Task<Model.TvSeries> GetById(string id, CancellationToken cancellationToken)
  {
    return _apiManager.GetTvSeriesById(id, cancellationToken);
  }

  /// <inheritdoc />
  protected async override Task<IMetadataResult<Series, SeriesInfo>> GetDetailById(string id, CancellationToken cancellationToken)
  {
    return await _apiManager.GetTvSeriesDetailById(id, cancellationToken).ConfigureAwait(false);
  }

  /// <inheritdoc />
  protected override Task<IEnumerable<Model.TvSeries>> Search(SeriesInfo info, CancellationToken cancellationToken)
  {
    return _apiManager.SearchTvSeries(info.Name, info.Year, cancellationToken);
  }

  public override IEnumerable<ImageType> GetSupportedImages(BaseItem item) => new[] { ImageType.Primary, ImageType.Backdrop, ImageType.Banner, ImageType.Logo };
}

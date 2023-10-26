using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaDataHub.Plugin.Api;
using MediaDataHub.Plugin.Api.Manager;
using Model = MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Provider.BoxSets;

public class BoxSetProvider : MediaDataHubProvider<Model.Collection, BoxSet, BoxSetInfo>
{
  private readonly MediaDataHubApiManager _apiManager;

  public BoxSetProvider(
    ILogger<BoxSetProvider> logger,
    MediaDataHubApiManager apiManager,
    IHttpClientFactory httpClientFactory
  ) : base(logger, httpClientFactory)
  {
    _apiManager = apiManager;
  }

  /// <inheritdoc />
  protected override Task<Model.Collection> GetById(string id, CancellationToken cancellationToken)
  {
    return _apiManager.GetCollectionById(id, cancellationToken);
  }

  /// <inheritdoc />
  protected async override Task<IMetadataResult<BoxSet, BoxSetInfo>> GetDetailById(string id, CancellationToken cancellationToken)
  {
    return await _apiManager.GetCollectionDetailById(id, cancellationToken).ConfigureAwait(false);
  }

  /// <inheritdoc />
  protected override Task<IEnumerable<Model.Collection>> Search(BoxSetInfo info, CancellationToken cancellationToken)
  {
    return _apiManager.SearchCollections(info.Name, cancellationToken);
  }

  public override IEnumerable<ImageType> GetSupportedImages(BaseItem item) => new[] { ImageType.Primary, ImageType.Backdrop };
}

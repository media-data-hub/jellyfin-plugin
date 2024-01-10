using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using MediaDataHub.Plugin.Api;
using Model = MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Provider;

public abstract class MediaDataHubBaseProvider<TRecordType, TItemType, TLookupInfoType> : IRemoteMetadataProvider<TItemType, TLookupInfoType>, IRemoteImageProvider
  where TRecordType : IRemoteSearchResult, IRemoteImageInfo
  where TItemType : BaseItem, IHasLookupInfo<TLookupInfoType>, new()
  where TLookupInfoType : ItemLookupInfo, new()
{
  protected readonly IHttpClientFactory _httpClientFactory;
  protected readonly ILogger _logger;

  public MediaDataHubBaseProvider(ILogger logger, IHttpClientFactory httpClientFactory)
  {
    _logger = logger;
    _httpClientFactory = httpClientFactory;
  }

  /// <inheritdoc />
  public string Name => Plugin.ProviderName;

  /// <inheritdoc />
  public abstract Task<IEnumerable<RemoteSearchResult>> GetSearchResults(TLookupInfoType info, CancellationToken cancellationToken);

  /// <inheritdoc />
  public abstract Task<MetadataResult<TItemType>> GetMetadata(TLookupInfoType info, CancellationToken cancellationToken);

  protected abstract Task<TRecordType> GetById(string id, CancellationToken cancellationToken);

  /// <inheritdoc />
  public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken) => _httpClientFactory.CreateClient().GetAsync(url, cancellationToken);

  public abstract IEnumerable<ImageType> GetSupportedImages(BaseItem item);

  public virtual async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
  {
    var list = new List<RemoteImageInfo>();
    if (item is not TItemType)
    {
      return list;
    }
    if (!item.TryGetProviderId(Plugin.ProviderId, out var id))
    {
      return list;
    }
    try
    {
      var record = await GetById(id, cancellationToken).ConfigureAwait(false);
      list.AddRange(record.ToRemoteImageInfo());
    }
    catch (Model.ApiException e)
    {
      _logger.LogWarning(e, "Failed to load image for boxSet ({id})", id);
    }
    return list;
  }

  public bool Supports(BaseItem item) => item is TItemType;
}

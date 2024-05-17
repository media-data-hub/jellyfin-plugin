using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using MediaDataHub.Plugin.Api;
using Model = MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Provider;

public abstract class MediaDataHubProvider<TRecordType, TItemType, TLookupInfoType> : MediaDataHubBaseProvider<TRecordType, TItemType, TLookupInfoType>
  where TRecordType : IRemoteSearchResult, IRemoteImageInfo
  where TItemType : BaseItem, IHasLookupInfo<TLookupInfoType>, new()
  where TLookupInfoType : ItemLookupInfo, new()
{

  public MediaDataHubProvider(ILogger logger, IHttpClientFactory httpClientFactory) : base(logger, httpClientFactory)
  {
  }

  /// <inheritdoc />
  public override async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(TLookupInfoType info, CancellationToken cancellationToken)
  {
    if (info.TryGetProviderId(Plugin.ProviderId, out var id))
    {
      _logger.LogInformation("GetSearchResults ({id})", id);
      var record = await GetById(id, cancellationToken).ConfigureAwait(false);
      if (record is not null)
      {
        var remoteResult = record.ToRemoteSearchResult();
        return [remoteResult];
      }
    }
    _logger.LogInformation("GetSearchResults ({name})", info.Name);
    var results = await Search(info, cancellationToken).ConfigureAwait(false);
    return results.Select(record => record.ToRemoteSearchResult());
  }

  /// <inheritdoc />
  public override async Task<MetadataResult<TItemType>> GetMetadata(TLookupInfoType info, CancellationToken cancellationToken)
  {
    var id = info.GetProviderId(Plugin.ProviderId);
    if (string.IsNullOrEmpty(id))
    {
      var results = await Search(info, cancellationToken).ConfigureAwait(false);
      var result = results.FirstOrDefault();
      if (result == null)
      {
        _logger.LogInformation("GetMetadata (No Id)");
        return new MetadataResult<TItemType>();
      }
      id = result.Id;
    }
    _logger.LogInformation("GetMetadata ({id})", id);
    try
    {
      var detail = await GetDetailById(id, cancellationToken).ConfigureAwait(false);
      return detail.ToMetadataResult(info);
    }
    catch (Model.ApiException e)
    {
      _logger.LogWarning(e, "Failed to load ({id})", id);
      return new MetadataResult<TItemType>();
    }
  }

  protected abstract Task<IMetadataResult<TItemType, TLookupInfoType>> GetDetailById(string id, CancellationToken cancellationToken);

  protected abstract Task<IEnumerable<TRecordType>> Search(TLookupInfoType info, CancellationToken cancellationToken);
}

using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using MediaDataHub.Plugin.Api;
using MediaDataHub.Plugin.Api.Manager;
using Model = MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Provider;

public class StudioImageProvider : IRemoteImageProvider
{
  private readonly MediaDataHubApiManager _apiManager;
  protected readonly IHttpClientFactory _httpClientFactory;
  protected readonly ILogger _logger;

  public StudioImageProvider(
    ILogger logger,
    IHttpClientFactory httpClientFactory,
    MediaDataHubApiManager apiManager
  )
  {
    _logger = logger;
    _httpClientFactory = httpClientFactory;
    _apiManager = apiManager;
  }

  /// <inheritdoc />
  public string Name => Plugin.ProviderName;

  /// <inheritdoc />
  public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken) => _httpClientFactory.CreateClient().GetAsync(url, cancellationToken);

  public IEnumerable<ImageType> GetSupportedImages(BaseItem item) => new[] { ImageType.Primary, ImageType.Backdrop, ImageType.Banner, ImageType.Logo, ImageType.Thumb };

  public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
  {
    if (item is not Studio)
    {
      return Enumerable.Empty<RemoteImageInfo>();
    }
    try
    {
      if (item.TryGetProviderId(Plugin.ProviderId, out var id))
      {
        var list = new List<RemoteImageInfo>();
        IRemoteImageInfo record = await GetById(id, cancellationToken).ConfigureAwait(false);
        list.AddRange(record.ToRemoteImageInfo());
        return list;
      }
      else
      {
        IEnumerable<IRemoteImageInfo> records = await _apiManager.SearchStudios(item.Name, cancellationToken);
        return records.FirstOrDefault()?.ToRemoteImageInfo() ?? Enumerable.Empty<RemoteImageInfo>();
      }
    }
    catch (Model.ApiException e)
    {
      _logger.LogWarning(e, "Failed to load image for studio");
    }
    return Enumerable.Empty<RemoteImageInfo>();
  }

  public bool Supports(BaseItem item) => item is Studio;
  protected Task<Model.Studio> GetById(string id, CancellationToken cancellationToken)
  {
    return _apiManager.GetStudioById(id, cancellationToken);
  }
}

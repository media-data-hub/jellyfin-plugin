using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using MediaDataHub.Plugin.Api;
using MediaDataHub.Plugin.Api.Manager;
using Model = MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Provider.Music;

public class ArtistProvider : MediaDataHubProvider<Model.Person, MusicArtist, ArtistInfo>
{
  private readonly MediaDataHubApiManager _apiManager;

  public ArtistProvider(
    ILogger<ArtistProvider> logger,
    MediaDataHubApiManager apiManager,
    IHttpClientFactory httpClientFactory
  ) : base(logger, httpClientFactory)
  {
    _apiManager = apiManager;
  }

  /// <inheritdoc />
  protected override Task<Model.Person> GetById(string id, CancellationToken cancellationToken)
  {
    return _apiManager.GetPersonById(id, cancellationToken);
  }

  /// <inheritdoc />
  protected async override Task<IMetadataResult<MusicArtist, ArtistInfo>> GetDetailById(string id, CancellationToken cancellationToken)
  {
    return await _apiManager.GetPersonDetailById(id, cancellationToken).ConfigureAwait(false);
  }

  /// <inheritdoc />
  protected override Task<IEnumerable<Model.Person>> Search(ArtistInfo info, CancellationToken cancellationToken)
  {
    return _apiManager.SearchPeople(info.Name, info.Year, cancellationToken);
  }

  public override IEnumerable<ImageType> GetSupportedImages(BaseItem item) => [ImageType.Primary, ImageType.Backdrop];

  public override async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
  {
    var list = new List<RemoteImageInfo>();
    if (item is not MusicArtist)
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
      var imgs = new Dictionary<ImageType, IEnumerable<string>>() {
        { ImageType.Primary, record.Thumbnails },
        { ImageType.Backdrop, record.Backdrop }
      };
      list.AddRange(imgs.SelectMany(
        img => img.Value.Select(fileName => new RemoteImageInfo
        {
          ProviderName = Plugin.ProviderName,
          Type = img.Key,
          Url = MediaDataHubUtils.GetFileUrl(record, fileName)
        })
      ));
    }
    catch (Model.ApiException e)
    {
      _logger.LogWarning(e, "Failed to load image for boxSet ({id})", id);
    }
    return list;
  }
}

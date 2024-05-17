using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaDataHub.Plugin.Api;
using MediaDataHub.Plugin.Api.Manager;
using Model = MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Provider.Music;

public class MusicAlbumProvider : MediaDataHubProvider<Model.MusicAlbum, MusicAlbum, AlbumInfo>
{
  private readonly MediaDataHubApiManager _apiManager;

  public MusicAlbumProvider(
    ILogger<MusicAlbumProvider> logger,
    MediaDataHubApiManager apiManager,
    IHttpClientFactory httpClientFactory
  ) : base(logger, httpClientFactory)
  {
    _apiManager = apiManager;
  }

  /// <inheritdoc />
  protected override Task<Model.MusicAlbum> GetById(string id, CancellationToken cancellationToken)
  {
    return _apiManager.GetMusicAlbumById(id, cancellationToken);
  }

  /// <inheritdoc />
  protected async override Task<IMetadataResult<MusicAlbum, AlbumInfo>> GetDetailById(string id, CancellationToken cancellationToken)
  {
    return await _apiManager.GetMusicAlbumDetailById(id, cancellationToken).ConfigureAwait(false);
  }

  /// <inheritdoc />
  protected override Task<IEnumerable<Model.MusicAlbum>> Search(AlbumInfo info, CancellationToken cancellationToken)
  {
    return _apiManager.SearchMusicAlbums(info.Name, info.Year, cancellationToken);
  }

  public override IEnumerable<ImageType> GetSupportedImages(BaseItem item) => [ImageType.Primary, ImageType.Backdrop, ImageType.Banner, ImageType.Logo, ImageType.Thumb];
}

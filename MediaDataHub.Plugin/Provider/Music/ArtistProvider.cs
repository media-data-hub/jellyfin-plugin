using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
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

  public override IEnumerable<ImageType> GetSupportedImages(BaseItem item) => new[] { ImageType.Primary, ImageType.Backdrop };
}

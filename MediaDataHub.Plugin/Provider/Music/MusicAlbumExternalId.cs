using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace MediaDataHub.Plugin.Provider.Music;

public class MusicAlbumExternalId : IExternalId
{
  /// <inheritdoc />
  public string ProviderName => Plugin.ProviderName;

  /// <inheritdoc />
  public string Key => Plugin.ProviderId;

  /// <inheritdoc />
  public ExternalIdMediaType? Type => ExternalIdMediaType.Album;

  /// <inheritdoc />
  public string? UrlFormatString => null;

  /// <inheritdoc />
  public bool Supports(IHasProviderIds item) => item is MusicAlbum;
}

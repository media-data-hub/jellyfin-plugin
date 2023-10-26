using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace MediaDataHub.Plugin.Provider.Movies;

public class MovieExternalId : IExternalId
{
  /// <inheritdoc />
  public string ProviderName => Plugin.ProviderName;

  /// <inheritdoc />
  public string Key => Plugin.ProviderId;

  /// <inheritdoc />
  public ExternalIdMediaType? Type => ExternalIdMediaType.Movie;

  /// <inheritdoc />
  public string? UrlFormatString => null;

  /// <inheritdoc />
  public bool Supports(IHasProviderIds item) => item is Movie;
}

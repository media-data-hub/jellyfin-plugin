using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace MediaDataHub.Plugin.Provider.BoxSets;

public class BoxSetExternalId : IExternalId
{
  /// <inheritdoc />
  public string ProviderName => Plugin.ProviderName;

  /// <inheritdoc />
  public string Key => Plugin.ProviderId;

  /// <inheritdoc />
  public ExternalIdMediaType? Type => ExternalIdMediaType.BoxSet;

  /// <inheritdoc />
  public string? UrlFormatString => null;

  /// <inheritdoc />
  public bool Supports(IHasProviderIds item) => item is BoxSet;
}

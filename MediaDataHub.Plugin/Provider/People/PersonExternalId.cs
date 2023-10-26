using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace MediaDataHub.Plugin.Provider.People;

public class PersonExternalId : IExternalId
{
  /// <inheritdoc />
  public string ProviderName => Plugin.ProviderName;

  /// <inheritdoc />
  public string Key => Plugin.ProviderId;

  /// <inheritdoc />
  public ExternalIdMediaType? Type => ExternalIdMediaType.Person;

  /// <inheritdoc />
  public string? UrlFormatString => null;

  /// <inheritdoc />
  public bool Supports(IHasProviderIds item) => item is Person;
}

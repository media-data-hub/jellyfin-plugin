using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaDataHub.Plugin.Api;
using MediaDataHub.Plugin.Api.Manager;
using Model = MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Provider.People;

public class PersonProvider : MediaDataHubProvider<Model.Person, Person, PersonLookupInfo>
{
  private readonly MediaDataHubApiManager _apiManager;

  public PersonProvider(
    ILogger<PersonProvider> logger,
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
  protected async override Task<IMetadataResult<Person, PersonLookupInfo>> GetDetailById(string id, CancellationToken cancellationToken)
  {
    return await _apiManager.GetPersonDetailById(id, cancellationToken).ConfigureAwait(false);
  }

  /// <inheritdoc />
  protected override Task<IEnumerable<Model.Person>> Search(PersonLookupInfo info, CancellationToken cancellationToken)
  {
    return _apiManager.SearchPeople(info.Name, info.Year, cancellationToken);
  }

  public override IEnumerable<ImageType> GetSupportedImages(BaseItem item) => [ImageType.Primary, ImageType.Backdrop];
}

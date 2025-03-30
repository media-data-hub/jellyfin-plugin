using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api.Manager;

public partial class MediaDataHubApiManager : IStudioApi
{
  public Task<Studio> GetStudioById(string id, CancellationToken cancellationToken)
  {
    return GetById<Studio>(Collections.Studio, id, cancellationToken);
  }

  public Task<IEnumerable<Studio>> SearchStudios(string name, CancellationToken cancellationToken)
  {
    name = escapeName(name);
    var filters = new List<string> { $"name='{name}' || sortName='{name}'" };
    return Search<Studio>(Collections.Studio, filters, "sortName", 100, cancellationToken);
  }
}

using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api.Manager;

public interface IStudioApi
{
  public Task<Studio> GetStudioById(string id, CancellationToken cancellationToken);
  public Task<IEnumerable<Studio>> SearchStudios(string name, CancellationToken cancellationToken);
}

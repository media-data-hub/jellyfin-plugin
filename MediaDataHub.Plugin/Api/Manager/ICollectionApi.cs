using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api.Manager;

public interface ICollectionApi
{
  public Task<Collection> GetCollectionById(string id, CancellationToken cancellationToken);
  public Task<CollectionDetail> GetCollectionDetailById(string id, CancellationToken cancellationToken);
  public Task<IEnumerable<Collection>> SearchCollections(string name, CancellationToken cancellationToken);
}

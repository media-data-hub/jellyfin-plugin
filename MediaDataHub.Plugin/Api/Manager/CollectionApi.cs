using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api.Manager;

public partial class MediaDataHubApiManager : ICollectionApi
{
  public Task<Collection> GetCollectionById(string id, CancellationToken cancellationToken)
  {
    return GetById<Collection>(Collections.Collection, id, cancellationToken);
  }

  public Task<CollectionDetail> GetCollectionDetailById(string id, CancellationToken cancellationToken)
  {
    return GetDetailById<CollectionDetail>(Collections.Collection, CollectionDetail.Query, id, cancellationToken);
  }

  public Task<IEnumerable<Collection>> SearchCollections(string name, CancellationToken cancellationToken)
  {
    var filters = new List<string> {
      $"name='{name}' || sortName='{name}'",
      $"name~'{name}' || sortName~'{name}'",
    };
    var nameTokens = name.Split();
    if (nameTokens.Length > 1)
    {
      var filter = string.Join(" || ", nameTokens.Select(token => $"name='{token}' || sortName='{token}'"));
      filters.Add(filter);
    }
    return Search<Collection>(Collections.Collection, filters, "sortName", 100, cancellationToken);
  }
}

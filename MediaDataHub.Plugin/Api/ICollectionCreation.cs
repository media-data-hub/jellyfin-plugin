using MediaBrowser.Controller.Collections;

namespace MediaDataHub.Plugin.Api;

public interface ICollectionCreation : IRecord
{
  public string Name { get; }

  public CollectionCreationOptions ToCollectionCreationOptions()
  {
    return new CollectionCreationOptions
    {
      Name = Name,
      ProviderIds = new Dictionary<string, string> { { Plugin.ProviderId, Id } }
    };
  }
}

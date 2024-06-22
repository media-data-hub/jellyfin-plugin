using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace MediaDataHub.Plugin.Api;

public interface IRemoteImageInfo : IRecord
{
  public Dictionary<ImageType, IEnumerable<string>> RemoteImages => [];

  public IEnumerable<RemoteImageInfo> ToRemoteImageInfo()
  {
    return RemoteImages.SelectMany(
      img => img.Value.Select(fileName => new RemoteImageInfo
      {
        ProviderName = Plugin.ProviderName,
        Type = img.Key,
        Url = MediaDataHubUtils.GetFileUrl(this, fileName)
      })
    );
  }
}

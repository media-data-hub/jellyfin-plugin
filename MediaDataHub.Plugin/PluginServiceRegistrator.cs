using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;
using MediaDataHub.Plugin.Api;
using MediaDataHub.Plugin.Api.Manager;
using MediaDataHub.Plugin.Version;

namespace MediaDataHub.Plugin
{
  /// <inheritdoc />
  public class PluginServiceRegistrator : IPluginServiceRegistrator
  {
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
    {
      serviceCollection.AddSingleton<MediaDataHubApiClient>();
      serviceCollection.AddSingleton<MediaDataHubApiManager>();
      serviceCollection.AddSingleton<VersionManager>();
    }
  }
}

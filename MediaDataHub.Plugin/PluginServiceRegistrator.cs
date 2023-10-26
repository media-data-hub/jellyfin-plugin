using MediaBrowser.Common.Plugins;
using Microsoft.Extensions.DependencyInjection;
using MediaDataHub.Plugin.Api;
using MediaDataHub.Plugin.Api.Manager;

namespace MediaDataHub.Plugin
{
  /// <inheritdoc />
  public class PluginServiceRegistrator : IPluginServiceRegistrator
  {
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection serviceCollection)
    {
      serviceCollection.AddSingleton<MediaDataHubApiClient>();
      serviceCollection.AddSingleton<MediaDataHubApiManager>();
    }
  }
}

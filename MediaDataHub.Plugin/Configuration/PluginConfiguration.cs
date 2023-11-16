using MediaBrowser.Model.Plugins;

namespace MediaDataHub.Plugin.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
  public string Host { get; set; } = "";
  public string Identity { get; set; } = "";
  public string Password { get; set; } = "";
  public bool AutoAddToCollection { get; set; } = true;
  public bool AutoConnectCollection { get; set; } = false;
  public bool AutoCreateCollection { get; set; } = true;
  public bool AutoRefreshCollection { get; set; } = false;
}

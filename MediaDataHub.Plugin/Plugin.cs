using System.Globalization;
using MediaDataHub.Plugin.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace MediaDataHub.Plugin;

public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
  public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
      : base(applicationPaths, xmlSerializer)
  {
    Instance = this;
  }
  public static Plugin? Instance { get; private set; }
  public static readonly string ProviderName = "MediaDataHub";
  public static readonly string ProviderId = "MediaDataHub";

  /// <inheritdoc />
  public override string Name => ProviderName;

  /// <inheritdoc />
  public override Guid Id => Guid.Parse("88ce23bd-f56f-4269-9949-e734326e9797");

  /// <inheritdoc />
  public IEnumerable<PluginPageInfo> GetPages()
  {
    return
    [
      new PluginPageInfo
      {
        Name = Name,
        EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html", GetType().Namespace)
      }
    ];
  }
}

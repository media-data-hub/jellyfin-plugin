using Microsoft.Extensions.Logging;
using MediaBrowser.Model.Tasks;
using MediaDataHub.Plugin.Version;

namespace MediaDataHub.Plugin.ScheduledTask;

public class SplitEpisodesTask : IScheduledTask
{
  private readonly ILogger<SplitEpisodesTask> _logger;
  private readonly VersionManager _versionManager;
  public string Name => "Split All Episodes";
  public string Key => "MediaDataHub.SplitEpisodesTask";
  public string Description => "Scans all libraries to Split repeated Episodes";
  public string Category => "Media Data Hub";

  public SplitEpisodesTask(
    VersionManager versionManager,
    ILogger<SplitEpisodesTask> logger
  )
  {
    _versionManager = versionManager;
    _logger = logger;
  }

  public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Start Scheduled Task ({name})", Name);
    _versionManager.SplitEpisodes(progress);
    _logger.LogInformation("End Scheduled Task ({name})", Name);
    return Task.CompletedTask;
  }

  public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
  {
    // Run this task every 24 hours
    return Array.Empty<TaskTriggerInfo>();
  }
}

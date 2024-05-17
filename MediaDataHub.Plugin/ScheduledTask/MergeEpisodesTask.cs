using Microsoft.Extensions.Logging;
using MediaBrowser.Model.Tasks;
using MediaDataHub.Plugin.Version;

namespace MediaDataHub.Plugin.ScheduledTask;

public class MergeEpisodesTask : IScheduledTask
{
  private readonly ILogger<MergeEpisodesTask> _logger;
  private readonly VersionManager _versionManager;
  public string Name => "Merge All Episodes";
  public string Key => "MediaDataHub.MergeEpisodesTask";
  public string Description => "Scans all libraries to merge repeated Episodes";
  public string Category => "Media Data Hub";

  public MergeEpisodesTask(
    VersionManager versionManager,
    ILogger<MergeEpisodesTask> logger
  )
  {
    _versionManager = versionManager;
    _logger = logger;
  }

  public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Start Scheduled Task ({name})", Name);
    _versionManager.MergeEpisodes(progress);
    _logger.LogInformation("End Scheduled Task ({name})", Name);
    return Task.CompletedTask;
  }

  public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
  {
    // Run this task every 24 hours
    return [];
  }
}

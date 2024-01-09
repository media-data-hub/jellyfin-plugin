using Microsoft.Extensions.Logging;
using MediaBrowser.Model.Tasks;
using MediaDataHub.Plugin.Version;

namespace MediaDataHub.Plugin.ScheduledTask;

public class MergeMoviesTask : IScheduledTask
{
  private readonly ILogger<MergeMoviesTask> _logger;
  private readonly VersionManager _versionManager;
  public string Name => "Merge All Movies";
  public string Key => "MediaDataHub.MergeMoviesTask";
  public string Description => "Scans all libraries to merge repeated movies";
  public string Category => "Media Data Hub";

  public MergeMoviesTask(
    VersionManager versionManager,
    ILogger<MergeMoviesTask> logger
  )
  {
    _versionManager = versionManager;
    _logger = logger;
  }

  public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Start Scheduled Task ({name})", Name);
    _versionManager.MergeMovies(progress);
    _logger.LogInformation("End Scheduled Task ({name})", Name);
    return Task.CompletedTask;
  }

  public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
  {
    // Run this task every 24 hours
    return Array.Empty<TaskTriggerInfo>();
  }
}

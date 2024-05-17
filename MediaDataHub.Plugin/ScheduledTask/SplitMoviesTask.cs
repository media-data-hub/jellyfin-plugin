using Microsoft.Extensions.Logging;
using MediaBrowser.Model.Tasks;
using MediaDataHub.Plugin.Version;

namespace MediaDataHub.Plugin.ScheduledTask;

public class SplitMoviesTask : IScheduledTask
{
  private readonly ILogger<SplitMoviesTask> _logger;
  private readonly VersionManager _versionManager;
  public string Name => "Split All Movies";
  public string Key => "MediaDataHub.SplitMoviesTask";
  public string Description => "Scans all libraries to Split repeated movies";
  public string Category => "Media Data Hub";

  public SplitMoviesTask(
    VersionManager versionManager,
    ILogger<SplitMoviesTask> logger
  )
  {
    _versionManager = versionManager;
    _logger = logger;
  }

  public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Start Scheduled Task ({name})", Name);
    _versionManager.SplitMovies(progress);
    _logger.LogInformation("End Scheduled Task ({name})", Name);
    return Task.CompletedTask;
  }

  public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
  {
    // Run this task every 24 hours
    return [];
  }
}

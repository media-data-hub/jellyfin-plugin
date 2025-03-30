using Microsoft.Extensions.Logging;
using System.Web;

namespace MediaDataHub.Plugin.Api.Manager;

public partial class MediaDataHubApiManager : IDisposable
{
  private readonly ILogger<MediaDataHubApiManager> _logger;
  private readonly MediaDataHubApiClient _client;
  private bool disposed = false;

  public MediaDataHubApiManager(ILogger<MediaDataHubApiManager> logger, MediaDataHubApiClient client)
  {
    _logger = logger;
    _client = client;
  }

  ~MediaDataHubApiManager()
  {
    Dispose(false);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (disposed)
      return;
    if (disposing)
    {
      _client.Dispose();
    }
    disposed = true;
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  protected string escapeName(string name)
  {
    return HttpUtility.UrlEncode(name.Replace("'", "\\'")); ;
  }
}

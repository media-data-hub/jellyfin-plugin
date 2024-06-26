
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace MediaDataHub.Plugin.Configuration;

[ApiController]
[Authorize]
[Route("Plugin/MediaDataHub")]
[Produces(MediaTypeNames.Application.Json)]
public class ConfigurationController : ControllerBase
{
  private readonly ILogger<ConfigurationController> _logger;

  public ConfigurationController(ILogger<ConfigurationController> logger)
  {
    _logger = logger;
  }
}



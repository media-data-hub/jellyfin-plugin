using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaDataHub.Plugin.Api.Model;

public class ValidationResponse
{
  [JsonPropertyName("message")]
  public string Message { get; set; } = "";
}

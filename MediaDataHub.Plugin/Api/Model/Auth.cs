using System.Text.Json.Serialization;

namespace MediaDataHub.Plugin.Api.Model;

public class Auth : Record
{
  [JsonPropertyName("token")]
  public string Token { get; set; } = "";
}

using System.Text.Json.Serialization;

namespace MediaDataHub.Plugin.Api.Model;

public class AuthWithPasswordRequest : Record
{
  [JsonPropertyName("identity")]
  public string Identity { get; set; } = "";

  [JsonPropertyName("password")]
  public string Password { get; set; } = "";
}

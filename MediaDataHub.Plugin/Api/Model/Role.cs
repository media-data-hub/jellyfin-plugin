using System.Text.Json.Serialization;

namespace MediaDataHub.Plugin.Api.Model;

public class Role : Record
{
  [JsonPropertyName("name")]
  public string Name { get; set; } = "";

  [JsonPropertyName("jellyfin")]
  public string Jellyfin { get; set; } = "";
}

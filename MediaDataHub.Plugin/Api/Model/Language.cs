using System.Text.Json.Serialization;

namespace MediaDataHub.Plugin.Api.Model;

public class Language : Record
{
  [JsonPropertyName("name")]
  public string Name { get; set; } = "";

  [JsonPropertyName("iso639_1")]
  public string Iso639_1 { get; set; } = "";

  [JsonPropertyName("iso639_2")]
  public string Iso639_2 { get; set; } = "";
}

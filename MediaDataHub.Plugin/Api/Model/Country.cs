using System.Text.Json.Serialization;

namespace MediaDataHub.Plugin.Api.Model;

public class Country : Record
{
  [JsonPropertyName("name")]
  public string Name { get; set; } = "";

  [JsonPropertyName("alpha2")]
  public string Alpha2 { get; set; } = "";

  [JsonPropertyName("alpha3")]
  public string Alpha3 { get; set; } = "";
}

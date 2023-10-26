using System.Text.Json.Serialization;
using MediaDataHub.Plugin.Json;

namespace MediaDataHub.Plugin.Api.Model;

public class Studio : Record
{
  [JsonPropertyName("name")]
  public string Name { get; set; } = "";

  [JsonPropertyName("sortName")]
  public string SortName { get; set; } = "";

  [JsonPropertyName("description")]
  public string Description { get; set; } = "";

  [JsonPropertyName("language")]
  public string Language { get; set; } = "";

  [JsonPropertyName("country")]
  public string Country { get; set; } = "";

  [JsonPropertyName("foundedAt")]
  [JsonConverter(typeof(NullableDateTimeFormatConverter))]
  public DateTime? FoundedAt { get; set; }

  [JsonPropertyName("logo")]
  public string Logo { get; set; } = "";
}

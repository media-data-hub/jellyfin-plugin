using System.Text.Json.Serialization;
using MediaDataHub.Plugin.Json;

namespace MediaDataHub.Plugin.Api.Model;

public class Record : IRecord
{
  [JsonPropertyName("id")]
  public string Id { get; set; } = "";

  [JsonPropertyName("collectionId")]
  public string MetaCollectionId { get; set; } = "";

  [JsonPropertyName("collectionName")]
  public string MetaCollectionName { get; set; } = "";

  [JsonPropertyName("created")]
  [JsonConverter(typeof(DateTimeFormatConverter))]
  public DateTime CreatedAt { get; set; }

  [JsonPropertyName("updated")]
  [JsonConverter(typeof(DateTimeFormatConverter))]
  public DateTime UpdatedAt { get; set; }
}

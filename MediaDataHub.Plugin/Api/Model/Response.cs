using System.Text.Json.Serialization;
using MediaDataHub.Plugin.Json;

namespace MediaDataHub.Plugin.Api.Model;

public class Response<T>
{
  [JsonPropertyName("page")]
  public int Page { get; set; }

  [JsonPropertyName("perPage")]
  public int PerPage { get; set; }

  [JsonPropertyName("totalPages")]
  public int TotalPages { get; set; }

  [JsonPropertyName("totalItems")]
  public int TotalItems { get; set; }

  [JsonPropertyName("items")]
  public IEnumerable<T> Items { get; set; } = Array.Empty<T>();
}

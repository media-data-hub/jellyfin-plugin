using System.Text.Json.Serialization;
using MediaBrowser.Model.Entities;
using MediaDataHub.Plugin.Json;

namespace MediaDataHub.Plugin.Api.Model;

public class Studio : Record, IRemoteImageInfo
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

  [JsonPropertyName("posters")]
  public IEnumerable<string> Posters { get; set; } = [];

  [JsonPropertyName("backdrop")]
  public IEnumerable<string> Backdrop { get; set; } = [];

  [JsonPropertyName("banners")]
  public IEnumerable<string> Banners { get; set; } = [];

  [JsonPropertyName("logos")]
  public IEnumerable<string> Logos { get; set; } = [];

  [JsonPropertyName("thumbnails")]
  public IEnumerable<string> Thumbnails { get; set; } = [];
  public IEnumerable<string> ImageUrls => Posters;
  public Dictionary<ImageType, IEnumerable<string>> RemoteImages => new() {
    { ImageType.Primary, Posters },
    { ImageType.Backdrop, Backdrop },
    { ImageType.Banner, Banners },
    { ImageType.Logo, Logos },
    { ImageType.Thumb, Thumbnails }
  };
}

using System.Text.Json.Serialization;
using Entities = MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaDataHub.Plugin.Json;

namespace MediaDataHub.Plugin.Api.Model;

public class TvEpisode : Record, IRemoteSearchResult, IRemoteImageInfo
{
  [JsonPropertyName("name")]
  public string Name { get; set; } = "";

  [JsonPropertyName("sortName")]
  public string SortName { get; set; } = "";

  [JsonPropertyName("description")]
  public string Description { get; set; } = "";

  [JsonPropertyName("airDate")]
  [JsonConverter(typeof(DateTimeFormatConverter))]
  public DateTime AirDate { get; set; }

  [JsonPropertyName("rating")]
  public float Rating { get; set; }

  [JsonPropertyName("homepage")]
  public string Homepage { get; set; } = "";

  [JsonPropertyName("language")]
  public string LanguageId { get; set; } = "";

  [JsonPropertyName("country")]
  public string CountryId { get; set; } = "";

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

  [JsonPropertyName("order")]
  public int Order { get; set; }

  [JsonPropertyName("tvSeason")]
  public string TvSeasonId { get; set; } = "";
  public IEnumerable<string> ImageUrls => Posters;
  public string? Overview => Description;
  public DateTime? PremiereDate => AirDate;
  public int? ProductionYear => AirDate.Year;
  public Dictionary<ImageType, IEnumerable<string>> RemoteImages => new() {
    { ImageType.Primary, Posters },
    { ImageType.Backdrop, Backdrop },
    { ImageType.Banner, Banners },
    { ImageType.Logo, Logos },
    { ImageType.Thumb, Thumbnails }
  };
}

public class TvEpisodeExpand
{
  [JsonPropertyName("language")]
  public Language Language { get; set; } = new();

  [JsonPropertyName("country")]
  public Country Country { get; set; } = new();
}

public class TvEpisodeDetail : TvEpisode, IMetadataResult<Entities.Episode, EpisodeInfo>, IDetailStaff
{
  [JsonIgnore]
  public static readonly IDictionary<string, string?> Query = new Dictionary<string, string?>() { { "expand", "language,country" } };

  [JsonPropertyName("expand")]
  public TvEpisodeExpand Expand { get; set; } = new();

  [JsonIgnore]
  public IEnumerable<StaffDetail> Staff { get; set; } = [];
  public string ForcedSortName => SortName;
  public string[] ProductionLocations => [Expand.Country.Name];
  public float? CommunityRating => Convert.ToSingle(Rating);
  public IEnumerable<IPersonInfo> People => Staff;
  public int? IndexNumber => Order;
}

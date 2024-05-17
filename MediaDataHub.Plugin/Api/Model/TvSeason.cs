using System.Text.Json.Serialization;
using Entities = MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaDataHub.Plugin.Json;

namespace MediaDataHub.Plugin.Api.Model;

public class TvSeason : Record, IRemoteSearchResult, IRemoteImageInfo
{
  [JsonPropertyName("name")]
  public string Name { get; set; } = "";

  [JsonPropertyName("sortName")]
  public string SortName { get; set; } = "";

  [JsonPropertyName("description")]
  public string Description { get; set; } = "";

  [JsonPropertyName("tagline")]
  public string Tagline { get; set; } = "";

  [JsonPropertyName("airDate")]
  [JsonConverter(typeof(DateTimeFormatConverter))]
  public DateTime AirDate { get; set; }

  [JsonPropertyName("contentRatings")]
  public string ContentRatings { get; set; } = "";

  [JsonPropertyName("rating")]
  public float Rating { get; set; }

  [JsonPropertyName("homepage")]
  public string Homepage { get; set; } = "";

  [JsonPropertyName("language")]
  public string LanguageId { get; set; } = "";

  [JsonPropertyName("country")]
  public string CountryId { get; set; } = "";

  [JsonPropertyName("studios")]
  public IEnumerable<string> StudioIds { get; set; } = [];

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

  [JsonPropertyName("tvSeries")]
  public string TvSeriesId { get; set; } = "";
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

public class TvSeasonExpand
{
  [JsonPropertyName("language")]
  public Language Language { get; set; } = new();

  [JsonPropertyName("country")]
  public Country Country { get; set; } = new();

  [JsonPropertyName("studios")]
  public IEnumerable<Studio> Studios { get; set; } = [];
}

public class TvSeasonDetail : TvSeason, IMetadataResult<Entities.Season, SeasonInfo>, IDetailStaff
{
  [JsonIgnore]
  public static readonly IDictionary<string, string?> Query = new Dictionary<string, string?>() { { "expand", "language,country,studios" } };

  [JsonPropertyName("expand")]
  public TvSeasonExpand Expand { get; set; } = new();

  [JsonIgnore]
  public IEnumerable<StaffDetail> Staff { get; set; } = [];
  public string ForcedSortName => SortName;
  public string[] Studios => Expand.Studios.Select(tag => tag.Name).ToArray();
  public string[] ProductionLocations => [Expand.Country.Name];
  public string? OfficialRating => ContentRatings;
  public float? CommunityRating => Convert.ToSingle(Rating);
  public IEnumerable<IPersonInfo> People => Staff;
  public int? IndexNumber => Order;
}

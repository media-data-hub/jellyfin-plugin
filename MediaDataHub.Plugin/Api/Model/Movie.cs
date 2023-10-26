using System.Text.Json.Serialization;
using Entities = MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaDataHub.Plugin.Json;

namespace MediaDataHub.Plugin.Api.Model;

public class Movie : Record, IRemoteSearchResult, IRemoteImageInfo
{
  [JsonPropertyName("name")]
  public string Name { get; set; } = "";

  [JsonPropertyName("sortName")]
  public string SortName { get; set; } = "";

  [JsonPropertyName("matchName")]
  public string MatchName { get; set; } = "";

  [JsonPropertyName("description")]
  public string Description { get; set; } = "";

  [JsonPropertyName("tagline")]
  public string Tagline { get; set; } = "";

  [JsonPropertyName("releaseDate")]
  [JsonConverter(typeof(DateTimeFormatConverter))]
  public DateTime ReleaseDate { get; set; }

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

  [JsonPropertyName("collections")]
  public IEnumerable<string> CollectionIds { get; set; } = Array.Empty<string>();

  [JsonPropertyName("studios")]
  public IEnumerable<string> StudioIds { get; set; } = Array.Empty<string>();

  [JsonPropertyName("genres")]
  public IEnumerable<string> GenreIds { get; set; } = Array.Empty<string>();

  [JsonPropertyName("tags")]
  public IEnumerable<string> TagIds { get; set; } = Array.Empty<string>();

  [JsonPropertyName("posters")]
  public IEnumerable<string> Posters { get; set; } = Array.Empty<string>();

  [JsonPropertyName("backdrop")]
  public IEnumerable<string> Backdrop { get; set; } = Array.Empty<string>();

  [JsonPropertyName("banners")]
  public IEnumerable<string> Banners { get; set; } = Array.Empty<string>();

  [JsonPropertyName("logos")]
  public IEnumerable<string> Logos { get; set; } = Array.Empty<string>();
  protected IEnumerable<string> ImageUrls => Posters;
  public string? Overview => Description;
  public DateTime? PremiereDate => ReleaseDate;
  public int? ProductionYear => ReleaseDate.Year;
  public Dictionary<ImageType, IEnumerable<string>> RemoteImages => new() {
    { ImageType.Primary, Posters },
    { ImageType.Backdrop, Backdrop },
    { ImageType.Banner, Banners },
    { ImageType.Logo, Logos }
  };
}

public class MovieExpand
{
  [JsonPropertyName("language")]
  public Language Language { get; set; } = new();

  [JsonPropertyName("country")]
  public Country Country { get; set; } = new();

  [JsonPropertyName("collections")]
  public IEnumerable<Collection> Collections { get; set; } = Array.Empty<Collection>();

  [JsonPropertyName("studios")]
  public IEnumerable<Studio> Studios { get; set; } = Array.Empty<Studio>();

  [JsonPropertyName("genres")]
  public IEnumerable<Genre> Genres { get; set; } = Array.Empty<Genre>();

  [JsonPropertyName("tags")]
  public IEnumerable<Tag> Tags { get; set; } = Array.Empty<Tag>();
}

public class MovieDetail : Movie, IMetadataResult<Entities.Movie, MovieInfo>, IDetailStaff
{
  [JsonIgnore]
  public static readonly IDictionary<string, string?> Query = new Dictionary<string, string?>() { { "expand", "language,country,collections,studios,genres,tags" } };

  [JsonPropertyName("expand")]
  public MovieExpand Expand { get; set; } = new();

  [JsonIgnore]
  public IEnumerable<StaffDetail> Staff { get; set; } = Array.Empty<StaffDetail>();
  public string ForcedSortName => SortName;
  public string[] Tags => Expand.Tags.Select(tag => tag.Name).ToArray();
  public string[] Genres => Expand.Genres.Select(tag => tag.Name).ToArray();
  public string[] Studios => Expand.Studios.Select(tag => tag.Name).ToArray();
  public string[] ProductionLocations => new[] { Expand.Country.Name };
  public string? OfficialRating => ContentRatings;
  public float? CommunityRating => Convert.ToSingle(Rating);
  public IEnumerable<IPersonInfo> People => Staff;
}

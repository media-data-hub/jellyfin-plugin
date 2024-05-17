using System.Text.Json.Serialization;
using Entities = MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaDataHub.Plugin.Json;

namespace MediaDataHub.Plugin.Api.Model;

public class Collection : Record, IRemoteSearchResult, IRemoteImageInfo, ICollectionCreation
{
  [JsonPropertyName("name")]
  public string Name { get; set; } = "";

  [JsonPropertyName("sortName")]
  public string SortName { get; set; } = "";

  [JsonPropertyName("description")]
  public string Description { get; set; } = "";

  [JsonPropertyName("genres")]
  public IEnumerable<string> GenreIds { get; set; } = [];

  [JsonPropertyName("tags")]
  public IEnumerable<string> TagIds { get; set; } = [];

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

  [JsonPropertyName("releaseDate")]
  [JsonConverter(typeof(DateTimeFormatConverter))]
  public DateTime ReleaseDate { get; set; }

  [JsonPropertyName("contentRatings")]
  public string ContentRatings { get; set; } = "";

  [JsonPropertyName("rating")]
  public float Rating { get; set; }
  public IEnumerable<string> ImageUrls => Posters;
  public string? Overview => Description;
  public DateTime? PremiereDate => ReleaseDate;
  public int? ProductionYear => ReleaseDate.Year;
  public Dictionary<ImageType, IEnumerable<string>> RemoteImages => new() {
    { ImageType.Primary, Posters },
    { ImageType.Backdrop, Backdrop },
    { ImageType.Banner, Banners },
    { ImageType.Logo, Logos },
    { ImageType.Thumb, Thumbnails }
  };
}

public class CollectionExpand
{
  [JsonPropertyName("genres")]
  public IEnumerable<Genre> Genres { get; set; } = [];

  [JsonPropertyName("tags")]
  public IEnumerable<Tag> Tags { get; set; } = [];
}

public class CollectionDetail : Collection, IMetadataResult<Entities.BoxSet, BoxSetInfo>
{
  [JsonIgnore]
  public static readonly IDictionary<string, string?> Query = new Dictionary<string, string?>() { { "expand", "genres,tags" } };

  [JsonPropertyName("expand")]
  public CollectionExpand Expand { get; set; } = new();
  public string ForcedSortName => SortName;
  public string[] Tags => Expand.Tags.Select(tag => tag.Name).ToArray();
  public string[] Genres => Expand.Genres.Select(tag => tag.Name).ToArray();
  public string? OfficialRating => ContentRatings;
  public float? CommunityRating => Convert.ToSingle(Rating);
}

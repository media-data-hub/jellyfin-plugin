using System.Text.Json.Serialization;
using MediaBrowser.Controller.Entities.Audio;
using Entities = MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaDataHub.Plugin.Json;

namespace MediaDataHub.Plugin.Api.Model;

public class Person : Record, IRemoteSearchResult, IRemoteImageInfo
{
  [JsonPropertyName("name")]
  public string Name { get; set; } = "";

  [JsonPropertyName("sortName")]
  public string SortName { get; set; } = "";

  [JsonPropertyName("matchName")]
  public string MatchName { get; set; } = "";

  [JsonPropertyName("description")]
  public string Description { get; set; } = "";

  [JsonPropertyName("dob")]
  [JsonConverter(typeof(NullableDateTimeFormatConverter))]
  public DateTime? Dob { get; set; }

  [JsonPropertyName("dod")]
  [JsonConverter(typeof(NullableDateTimeFormatConverter))]
  public DateTime? Dod { get; set; }

  [JsonPropertyName("avatars")]
  public IEnumerable<string> Avatars { get; set; } = [];

  [JsonPropertyName("backdrop")]
  public IEnumerable<string> Backdrop { get; set; } = [];

  [JsonPropertyName("thumbnails")]
  public IEnumerable<string> Thumbnails { get; set; } = [];

  [JsonPropertyName("country")]
  public string CountryId { get; set; } = "";

  [JsonPropertyName("tags")]
  public IEnumerable<string> TagIds { get; set; } = [];
  public IEnumerable<string> ImageUrls => Avatars;
  public string? Overview => Description;
  public DateTime? PremiereDate => Dob;
  public int? ProductionYear => Dob?.Year;
  public Dictionary<ImageType, IEnumerable<string>> RemoteImages => new() {
    { ImageType.Primary, Avatars },
    { ImageType.Backdrop, Backdrop }
  };
}

public class PersonExpand
{
  [JsonPropertyName("country")]
  public Country Country { get; set; } = new();

  [JsonPropertyName("tags")]
  public IEnumerable<Tag> Tags { get; set; } = [];
}

public class PersonDetail : Person, IMetadataResult<Entities.Person, PersonLookupInfo>, IMetadataResult<MusicArtist, ArtistInfo>
{
  [JsonIgnore]
  public static readonly IDictionary<string, string?> Query = new Dictionary<string, string?>() { { "expand", "country,tags" } };

  [JsonPropertyName("expand")]
  public PersonExpand Expand { get; set; } = new();
  public string ForcedSortName => SortName;
  public DateTime? EndDate => Dod;
  public string[] Tags => Expand.Tags.Select(tag => tag.Name).ToArray();
  public string[] ProductionLocations => [Expand.Country.Name];
}

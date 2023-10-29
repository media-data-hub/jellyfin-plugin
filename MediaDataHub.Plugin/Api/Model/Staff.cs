using System.Text.Json.Serialization;

namespace MediaDataHub.Plugin.Api.Model;

public class Staff : Record
{
  [JsonPropertyName("role")]
  public string RoleId { get; set; } = "";

  [JsonPropertyName("person")]
  public string PersonId { get; set; } = "";

  [JsonPropertyName("priority")]
  public int Priority { get; set; }
}

public class StaffExpand
{
  [JsonPropertyName("role")]
  public Role Role { get; set; } = new();

  [JsonPropertyName("person")]
  public Person Person { get; set; } = new();
}

public class StaffDetail : Staff, IPersonInfo
{
  [JsonIgnore]
  public static readonly IDictionary<string, string?> Query = new Dictionary<string, string?>() { { "expand", "role,person" } };

  [JsonPropertyName("expand")]
  public StaffExpand Expand { get; set; } = new();
  public string Name => Expand.Person.Name;
  public string Role => Expand.Role.Name;
  public string Type => Expand.Role.Jellyfin;
  public int? SortOrder => Priority;
  public IEnumerable<string> ImageUrls => Expand.Person.Avatars;
  public string? ImageUrl
  {
    get
    {
      var url = ImageUrls.FirstOrDefault();
      return url != null ? MediaDataHubUtils.GetFileUrl(Expand.Person, url) : null;
    }
  }
  public Dictionary<string, string> ProviderIds => new() { { Plugin.ProviderId, Expand.Person.Id } };
}

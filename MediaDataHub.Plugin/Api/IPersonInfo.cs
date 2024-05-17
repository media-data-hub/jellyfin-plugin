using Jellyfin.Data.Enums;
using MediaBrowser.Controller.Entities;

namespace MediaDataHub.Plugin.Api;

public interface IPersonInfo : IRecord
{
  public string Name { get; }
  public string Role { get; }
  public string Type { get; }
  public int? SortOrder { get; }
  public IEnumerable<string> ImageUrls => Enumerable.Empty<string>();
  public string? ImageUrl
  {
    get
    {
      var url = ImageUrls.FirstOrDefault();
      return url != null ? MediaDataHubUtils.GetFileUrl(this, url) : null;
    }
  }
  public Dictionary<string, string> ProviderIds => new() { { Plugin.ProviderId, Id } };

  public PersonInfo ToPersonInfo()
  {
    var success = Enum.TryParse(Type, out PersonKind kind);
    if (!success)
    {
      kind = PersonKind.Unknown;
    }
    return new PersonInfo
    {
      Name = Name,
      Role = Role,
      Type = kind,
      SortOrder = SortOrder,
      ImageUrl = ImageUrl,
      ProviderIds = ProviderIds
    };
  }
}

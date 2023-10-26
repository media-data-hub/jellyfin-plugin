using MediaBrowser.Model.Providers;

namespace MediaDataHub.Plugin.Api;

public interface IRemoteSearchResult : IRecord
{
  public string SearchProviderName => Plugin.ProviderName;
  public string Name { get; }
  protected IEnumerable<string> ImageUrls => Enumerable.Empty<string>();
  public string? ImageUrl
  {
    get
    {
      var url = ImageUrls.FirstOrDefault();
      return url != null ? MediaDataHubUtils.GetFileUrl(this, url) : null;
    }
  }
  public string? Overview => null;
  public int? ProductionYear => null;
  public int? IndexNumber => null;
  public int? IndexNumberEnd => null;
  public int? ParentIndexNumber => null;
  public DateTime? PremiereDate => null;
  public Dictionary<string, string> ProviderIds => new() { { Plugin.ProviderId, Id } };

  public RemoteSearchResult ToRemoteSearchResult()
  {
    return new RemoteSearchResult
    {
      SearchProviderName = SearchProviderName,
      Name = Name,
      ImageUrl = ImageUrl,
      Overview = Overview,
      ProductionYear = ProductionYear,
      IndexNumber = IndexNumber,
      IndexNumberEnd = IndexNumberEnd,
      ParentIndexNumber = ParentIndexNumber,
      PremiereDate = PremiereDate,
      ProviderIds = ProviderIds,
    };
  }
}

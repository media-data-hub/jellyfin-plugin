using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;

namespace MediaDataHub.Plugin.Api;

public interface IMetadataResult<TItemType, TLookupInfoType> : IRecord
  where TItemType : BaseItem, IHasLookupInfo<TLookupInfoType>, new()
  where TLookupInfoType : ItemLookupInfo, new()
{
  public string Name { get; }
  public string OriginalTitle => Name;
  public string ForcedSortName { get; }
  public string? Overview => null;
  public string? Tagline => null;
  public string? OfficialRating => null;
  public float? CommunityRating => null;
  public float? CriticRating => null;
  public string[] Tags => Array.Empty<string>();
  public string[] Genres => Array.Empty<string>();
  public string[] Studios => Array.Empty<string>();
  public string[] ProductionLocations => Array.Empty<string>();
  public int? ProductionYear => null;
  public int? IndexNumber => null;
  public int? ParentIndexNumber => null;
  public DateTime? PremiereDate => null;
  public DateTime? EndDate => null;
  public Dictionary<string, string> ProviderIds => new() { { Plugin.ProviderId, Id } };
  public IEnumerable<IPersonInfo> People => Enumerable.Empty<IPersonInfo>();

  public MetadataResult<TItemType> ToMetadataResult(TLookupInfoType info)
  {
    var item = new TItemType
    {
      Name = Name,
      OriginalTitle = Name,
      ForcedSortName = ForcedSortName,
      Overview = Overview,
      Tagline = Tagline,
      OfficialRating = OfficialRating,
      CommunityRating = CommunityRating,
      CriticRating = CriticRating,
      Tags = Tags,
      Genres = Genres,
      Studios = Studios,
      ProductionLocations = ProductionLocations,
      ProductionYear = ProductionYear,
      IndexNumber = IndexNumber,
      ParentIndexNumber = ParentIndexNumber,
      PremiereDate = PremiereDate,
      EndDate = EndDate,
      ProviderIds = ProviderIds,
    };
    var result = new MetadataResult<TItemType>
    {
      HasMetadata = true,
      ResultLanguage = info.MetadataLanguage,
      Item = item
    };
    result.ResetPeople();
    foreach (var person in People)
    {
      result.AddPerson(person.ToPersonInfo());
    }
    return result;
  }
}

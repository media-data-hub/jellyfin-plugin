using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api.Manager;

public partial class MediaDataHubApiManager : IMusicAlbumApi
{
  public Task<MusicAlbum> GetMusicAlbumById(string id, CancellationToken cancellationToken)
  {
    return GetById<MusicAlbum>(Collections.MusicAlbum, id, cancellationToken);
  }

  public Task<MusicAlbumDetail> GetMusicAlbumDetailById(string id, CancellationToken cancellationToken)
  {
    return GetDetailById<MusicAlbumDetail>(Collections.MusicAlbum, MusicAlbumDetail.Query, id, cancellationToken);
  }

  public Task<IEnumerable<MusicAlbum>> SearchMusicAlbums(string name, int? year, CancellationToken cancellationToken)
  {
    name = escapeName(name);
    var filters = new List<string> {
      JoinReleaseDateFilter($"matchName='{name}'", year),
      JoinReleaseDateFilter($"name='{name}' || sortName='{name}'", year),
      JoinReleaseDateFilter($"name~'{name}' || sortName~'{name}'", year),
    };
    if (year != null && name.EndsWith($"({year})"))
    {
      var trim = name[..name.LastIndexOf($"({year})")].TrimEnd();
      filters.Insert(0, JoinReleaseDateFilter($"matchName='{trim}'", year));
    }
    var nameTokens = name.Split();
    if (nameTokens.Length > 1)
    {
      var filter = string.Join(" || ", nameTokens.Select(token => $"name='{token}' || sortName='{token}'"));
      filters.Add(JoinReleaseDateFilter(filter, year));
    }
    return Search<MusicAlbum>(Collections.MusicAlbum, filters, "sortName", 100, cancellationToken);
  }
}

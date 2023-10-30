using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api.Manager;

public interface IMusicAlbumApi
{
  public Task<MusicAlbum> GetMusicAlbumById(string id, CancellationToken cancellationToken);
  public Task<MusicAlbumDetail> GetMusicAlbumDetailById(string id, CancellationToken cancellationToken);
  public Task<IEnumerable<MusicAlbum>> SearchMusicAlbums(string name, int? year, CancellationToken cancellationToken);
}

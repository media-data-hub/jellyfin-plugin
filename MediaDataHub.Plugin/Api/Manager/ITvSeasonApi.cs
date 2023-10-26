using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api.Manager;

public interface ITvSeasonApi
{
  public Task<TvSeason> GetTvSeasonById(string id, CancellationToken cancellationToken);
  public Task<TvSeasonDetail> GetTvSeasonDetailById(string id, CancellationToken cancellationToken);
  public Task<TvSeasonDetail?> SearchTvSeason(string seriesId, int seasonNumber, CancellationToken cancellationToken);
}

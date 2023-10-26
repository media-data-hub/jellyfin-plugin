using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api.Manager;

public interface ITvEpisodeApi
{
  public Task<TvEpisode> GetTvEpisodeById(string id, CancellationToken cancellationToken);
  public Task<TvEpisodeDetail> GetTvEpisodeDetailById(string id, CancellationToken cancellationToken);
  public Task<TvEpisodeDetail?> SearchTvEpisode(string seriesId, int seasonNumber, int episodeNumber, CancellationToken cancellationToken);
}

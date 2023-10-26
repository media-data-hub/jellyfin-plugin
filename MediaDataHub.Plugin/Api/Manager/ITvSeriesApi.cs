using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api.Manager;

public interface ITvSeriesApi
{
  public Task<TvSeries> GetTvSeriesById(string id, CancellationToken cancellationToken);
  public Task<TvSeriesDetail> GetTvSeriesDetailById(string id, CancellationToken cancellationToken);
  public Task<IEnumerable<TvSeries>> SearchTvSeries(string name, int? year, CancellationToken cancellationToken);
}

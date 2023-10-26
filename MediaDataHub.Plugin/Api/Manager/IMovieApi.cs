using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api.Manager;

public interface IMovieApi
{
  public Task<Movie> GetMovieById(string id, CancellationToken cancellationToken);
  public Task<MovieDetail> GetMovieDetailById(string id, CancellationToken cancellationToken);
  public Task<IEnumerable<Movie>> SearchMovies(string name, int? year, CancellationToken cancellationToken);
}

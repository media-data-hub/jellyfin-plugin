using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api.Manager;

public partial class MediaDataHubApiManager : IMovieApi
{
  public Task<Movie> GetMovieById(string id, CancellationToken cancellationToken)
  {
    return GetById<Movie>(Collections.Movie, id, cancellationToken);
  }

  public Task<MovieDetail> GetMovieDetailById(string id, CancellationToken cancellationToken)
  {
    return GetDetailStaffById<MovieDetail>(Collections.Movie, Collections.MovieStaff, "movie", MovieDetail.Query, id, cancellationToken);
  }

  public Task<IEnumerable<Movie>> SearchMovies(string name, int? year, CancellationToken cancellationToken)
  {
    var filters = new List<string> {
      JoinReleaseDateFilter($"matchName='{name}'", year),
      JoinReleaseDateFilter($"name='{name}' || sortName='{name}'", year),
      JoinReleaseDateFilter($"name~'{name}' || sortName~'{name}'", year),
    };
    var nameTokens = name.Split();
    if (nameTokens.Length > 1)
    {
      var filter = string.Join(" || ", nameTokens.Select(token => $"name='{token}' || sortName='{token}'"));
      filters.Add(JoinReleaseDateFilter(filter, year));
    }
    return Search<Movie>(Collections.Movie, filters, "sortName", 100, cancellationToken);
  }

  private static string JoinReleaseDateFilter(string filter, int? year)
  {
    return year == null ? filter : $"({filter}) && (releaseDate>='{year}-01-01' && releaseDate<='{year}-12-31')";
  }
}

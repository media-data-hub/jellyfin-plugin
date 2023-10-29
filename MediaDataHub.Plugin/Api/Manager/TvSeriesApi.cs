using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api.Manager;

public partial class MediaDataHubApiManager : ITvSeriesApi
{
  public Task<TvSeries> GetTvSeriesById(string id, CancellationToken cancellationToken)
  {
    return GetById<TvSeries>(Collections.TvSeries, id, cancellationToken);
  }

  public Task<TvSeriesDetail> GetTvSeriesDetailById(string id, CancellationToken cancellationToken)
  {
    return GetDetailStaffById<TvSeriesDetail>(Collections.TvSeries, Collections.TvSeriesStaff, "tvSeries", TvSeriesDetail.Query, id, cancellationToken);
  }

  public Task<IEnumerable<TvSeries>> SearchTvSeries(string name, int? year, CancellationToken cancellationToken)
  {
    var filters = new List<string> {
      JoinFirstAirDateFilter($"matchName='{name}'", year),
      JoinFirstAirDateFilter($"name='{name}' || sortName='{name}'", year),
      JoinFirstAirDateFilter($"name~'{name}' || sortName~'{name}'", year),
    };
    if (year != null && name.EndsWith($"({year})"))
    {
      var trim = name[..name.LastIndexOf($"({year})")].TrimEnd();
      filters.Insert(0, JoinFirstAirDateFilter($"matchName='{trim}'", year));
    }
    var nameTokens = name.Split();
    if (nameTokens.Length > 1)
    {
      var filter = string.Join(" || ", nameTokens.Select(token => $"name='{token}' || sortName='{token}'"));
      filters.Add(JoinFirstAirDateFilter(filter, year));
    }
    return Search<TvSeries>(Collections.TvSeries, filters, "sortName", 100, cancellationToken);
  }

  private static string JoinFirstAirDateFilter(string filter, int? year)
  {
    return year == null ? filter : $"({filter}) && (firstAirDate>='{year}-01-01' && firstAirDate<='{year}-12-31')";
  }
}

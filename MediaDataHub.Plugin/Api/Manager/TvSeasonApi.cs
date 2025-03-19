using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api.Manager;

public partial class MediaDataHubApiManager : ITvSeasonApi
{
  public Task<TvSeason> GetTvSeasonById(string id, CancellationToken cancellationToken)
  {
    return GetById<TvSeason>(Collections.TvSeason, id, cancellationToken);
  }

  public Task<TvSeasonDetail> GetTvSeasonDetailById(string id, CancellationToken cancellationToken)
  {
    return GetDetailStaffById<TvSeasonDetail>(Collections.TvSeason, Collections.TvSeasonStaff, "tvSeason", TvSeasonDetail.Query, id, cancellationToken);
  }

  public async Task<TvSeasonDetail?> SearchTvSeason(string seriesId, int seasonNumber, CancellationToken cancellationToken)
  {
    var token = await _client.AuthWithPassword(cancellationToken).ConfigureAwait(false);
    var detailQuery = new Dictionary<string, string?>(TvSeasonDetail.Query) {
      {"filter", $"tvSeries.id='{seriesId}' && order={seasonNumber}"},
      {"skipTotal", "true"}
     };
    var detail = await _client.List<TvSeasonDetail>(Collections.TvSeason, cancellationToken, detailQuery, token);
    var season = detail.Items.FirstOrDefault();
    if (season == null)
    {
      return null;
    }
    var staffQuery = new Dictionary<string, string?>(StaffDetail.Query)
    {
      { "filter", $"tvSeason.id='{season.Id}' && role.jellyfin!=''" },
      { "sort", "priority" },
      { "perPage", "100" }
    };
    var staff = await _client.List<StaffDetail>(Collections.TvSeasonStaff, cancellationToken, staffQuery, token);
    season.Staff = staff.Items;
    return season;
  }
}

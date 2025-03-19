using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api.Manager;

public partial class MediaDataHubApiManager : ITvEpisodeApi
{
  public Task<TvEpisode> GetTvEpisodeById(string id, CancellationToken cancellationToken)
  {
    return GetById<TvEpisode>(Collections.TvEpisode, id, cancellationToken);
  }

  public Task<TvEpisodeDetail> GetTvEpisodeDetailById(string id, CancellationToken cancellationToken)
  {
    return GetDetailStaffById<TvEpisodeDetail>(Collections.TvEpisode, Collections.TvEpisodeStaff, "tvEpisode", TvEpisodeDetail.Query, id, cancellationToken);
  }

  public async Task<TvEpisodeDetail?> SearchTvEpisode(string seriesId, int seasonNumber, int episodeNumber, CancellationToken cancellationToken)
  {
    var token = await _client.AuthWithPassword(cancellationToken).ConfigureAwait(false);
    var detailQuery = new Dictionary<string, string?>(TvEpisodeDetail.Query) {
      {"filter", $"tvSeason.tvSeries.id='{seriesId}' && tvSeason.order={seasonNumber} && order={episodeNumber}"},
      {"skipTotal", "true"}
     };
    var detail = await _client.List<TvEpisodeDetail>(Collections.TvEpisode, cancellationToken, detailQuery, token).ConfigureAwait(false);
    var episode = detail.Items.FirstOrDefault();
    if (episode == null)
    {
      return null;
    }
    var staffQuery = new Dictionary<string, string?>(StaffDetail.Query)
    {
      { "filter", $"tvEpisode.id='{episode.Id}' && role.jellyfin!=''" },
      { "sort", "priority" },
      { "perPage", "100" }
    };
    var staff = await _client.List<StaffDetail>(Collections.TvEpisodeStaff, cancellationToken, staffQuery, token).ConfigureAwait(false);
    episode.Staff = staff.Items;
    return episode;
  }
  public async Task<IEnumerable<TvEpisodeDetail>> SearchTvEpisodes(string seriesId, int seasonNumber, int episodeNumberStart, int episodeNumberEnd, CancellationToken cancellationToken)
  {
    var token = await _client.AuthWithPassword(cancellationToken).ConfigureAwait(false);
    var detailQuery = new Dictionary<string, string?>(TvEpisodeDetail.Query) {
      {"filter", $"tvSeason.tvSeries.id='{seriesId}' && tvSeason.order={seasonNumber} && order>={episodeNumberStart} && order<={episodeNumberEnd}"},
      {"skipTotal", "true"}
     };
    var detail = await _client.List<TvEpisodeDetail>(Collections.TvEpisode, cancellationToken, detailQuery, token).ConfigureAwait(false);
    var episodes = detail.Items;
    foreach (var episode in episodes)
    {
      var staffQuery = new Dictionary<string, string?>(StaffDetail.Query)
      {
        { "filter", $"tvEpisode.id='{episode.Id}' && role.jellyfin!=''" },
        { "sort", "priority" },
        { "perPage", "100" }
      };
      var staff = await _client.List<StaffDetail>(Collections.TvEpisodeStaff, cancellationToken, staffQuery, token).ConfigureAwait(false);
      episode.Staff = staff.Items;
    }
    return episodes;
  }
}

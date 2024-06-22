using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api.Manager;

public partial class MediaDataHubApiManager
{
  protected async Task<T> GetById<T>(string metaCollectionName, string id, CancellationToken cancellationToken)
  {
    var token = await _client.AuthWithPassword(cancellationToken).ConfigureAwait(false);
    return await _client.View<T>(metaCollectionName, id, cancellationToken, null, token).ConfigureAwait(false);
  }

  protected async Task<T> GetDetailById<T>(string metaCollectionName, IDictionary<string, string?> query, string id, CancellationToken cancellationToken)
  {
    var token = await _client.AuthWithPassword(cancellationToken).ConfigureAwait(false);
    var detailQuery = new Dictionary<string, string?>(query);
    return await _client.View<T>(metaCollectionName, id, cancellationToken, detailQuery, token).ConfigureAwait(false);
  }

  protected async Task<T> GetDetailStaffById<T>(
      string detailMetaCollectionName,
      string staffMetaCollectionName,
      string staffDetailField,
      IDictionary<string, string?> query,
      string id,
      CancellationToken cancellationToken
  ) where T : IDetailStaff
  {
    var token = await _client.AuthWithPassword(cancellationToken).ConfigureAwait(false);
    var detailQuery = new Dictionary<string, string?>(query);
    var detail = await _client.View<T>(detailMetaCollectionName, id, cancellationToken, detailQuery, token).ConfigureAwait(false);
    var staffQuery = new Dictionary<string, string?>(StaffDetail.Query)
    {
      { "filter", $"{staffDetailField}.id='{id}' && role.jellyfin!=''" },
      { "sort", "priority" },
      { "perPage", "100" }
    };
    var staff = await _client.List<StaffDetail>(staffMetaCollectionName, cancellationToken, staffQuery, token).ConfigureAwait(false);
    detail.Staff = staff.Items.Select(v =>
    {
      switch (v.Type)
      {
        case "Actor":
          break;
        case "Director":
          v.Priority += 1000;
          break;
        case "Writer":
          v.Priority += 2000;
          break;
        default:
          v.Priority += 3000;
          break;
      }
      return v;
    });
    return detail;
  }

  protected async Task<IEnumerable<T>> Search<T>(string metaCollectionName, IEnumerable<string> filters, string sort, int maxCount, CancellationToken cancellationToken)
  where T : IRecord
  {
    var token = await _client.AuthWithPassword(cancellationToken).ConfigureAwait(false);
    List<T> records = [];
    foreach (var filter in filters)
    {
      var idFilter = string.Join(" && ", records.Select(record => $"id!='{record.Id}'"));
      var query = new Dictionary<string, string?>() {
      {"filter", records.Count > 0 ? $"{filter} && ({idFilter})" : filter},
      {"skipTotal", "true"},
      {"sort", sort},
      {"perPage", (maxCount - records.Count).ToString()}
     };
      var result = await _client.List<T>(metaCollectionName, cancellationToken, query, token).ConfigureAwait(false);
      records.AddRange(result.Items);
      if (records.Count >= maxCount)
      {
        return records.GetRange(0, maxCount);
      }
    }
    return records;
  }
}

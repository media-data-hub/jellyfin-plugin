using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api.Manager;

public partial class MediaDataHubApiManager : IPersonApi
{
  public Task<Person> GetPersonById(string id, CancellationToken cancellationToken)
  {
    return GetById<Person>(Collections.Person, id, cancellationToken);
  }

  public Task<PersonDetail> GetPersonDetailById(string id, CancellationToken cancellationToken)
  {
    return GetDetailById<PersonDetail>(Collections.Person, PersonDetail.Query, id, cancellationToken);
  }

  public Task<IEnumerable<Person>> SearchPeople(string name, int? year, CancellationToken cancellationToken)
  {
    name = name.Replace("'", "\\'");
    var filters = new List<string> {
      JoinDobFilter($"matchName='{name}'", year),
      JoinDobFilter($"name='{name}' || sortName='{name}'", year),
      JoinDobFilter($"name~'{name}' || sortName~'{name}'", year),
    };
    var nameTokens = name.Split();
    if (nameTokens.Length > 1)
    {
      var filter = string.Join(" || ", nameTokens.Select(token => $"name='{token}' || sortName='{token}'"));
      filters.Add(JoinDobFilter(filter, year));
    }
    return Search<Person>(Collections.Person, filters, "sortName", 100, cancellationToken);
  }

  private static string JoinDobFilter(string filter, int? year)
  {
    return year == null ? filter : $"({filter}) && ((dob>='{year}-01-01' && dob<='{year}-12-31') || dob='')";
  }
}

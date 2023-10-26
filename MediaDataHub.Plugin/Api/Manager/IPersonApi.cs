using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api.Manager;

public interface IPersonApi
{
  public Task<Person> GetPersonById(string id, CancellationToken cancellationToken);
  public Task<PersonDetail> GetPersonDetailById(string id, CancellationToken cancellationToken);
  public Task<IEnumerable<Person>> SearchPeople(string name, int? year, CancellationToken cancellationToken);
}

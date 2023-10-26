using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api;

public interface IDetailStaff
{
  public IEnumerable<StaffDetail> Staff { get; set; }
}

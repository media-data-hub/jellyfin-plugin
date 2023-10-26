namespace MediaDataHub.Plugin.Api;

public interface IRecord
{
  public string Id { get; set; }
  public string MetaCollectionId { get; set; }
  public string MetaCollectionName { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}

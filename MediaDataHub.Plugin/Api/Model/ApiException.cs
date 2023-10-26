using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaDataHub.Plugin.Api.Model;

[Serializable]
public class ApiException : Exception
{

  private record ValidationResponse
  {
    [JsonPropertyName("message")]
    public string Message { get; set; } = "";
  }

  public ApiException(string message) : base(message)
  {
  }

  public static ApiException FromResponse(HttpResponseMessage response)
  {
    var text = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
    if (text.Length > 0 && text[0] == '{')
    {
      var json = JsonSerializer.Deserialize<ValidationResponse>(text);
      return new ApiException(json?.Message ?? string.Empty);
    }
    return new ApiException(text);
  }
}

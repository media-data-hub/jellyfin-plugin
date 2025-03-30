using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Api;

public class MediaDataHubApiClient : IDisposable
{
  private readonly HttpClient _httpClient;

  private readonly ILogger<MediaDataHubApiClient> _logger;
  private bool disposed = false;

  public MediaDataHubApiClient(ILogger<MediaDataHubApiClient> logger)
  {
    _httpClient = new HttpClient
    {
      Timeout = TimeSpan.FromMinutes(1)
    };
    _logger = logger;
  }

  ~MediaDataHubApiClient()
  {
    Dispose(false);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (disposed)
      return;
    if (disposing)
    {
      _httpClient.Dispose();
    }
    disposed = true;
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  public async Task<string> AuthWithPassword(CancellationToken cancellationToken)
  {
    var config = MediaDataHubUtils.GetConfiguration();
    var host = MediaDataHubUtils.GetHost(config);
    if (string.IsNullOrEmpty(config.Identity))
    {
      throw new Exception("Identity cannot be empty");
    }
    if (string.IsNullOrEmpty(config.Password))
    {
      throw new Exception("Password cannot be empty");
    }

    var remoteUrl = string.Concat(host, "/api/collections/user/auth-with-password");
    using var requestMessage = new HttpRequestMessage(HttpMethod.Post, remoteUrl);
    var request = new AuthWithPasswordRequest
    {
      Identity = config.Identity,
      Password = config.Password
    };
    requestMessage.Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
    using var response = await _httpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
    if (response.StatusCode != HttpStatusCode.OK)
    {
      var text = response.Content.ReadAsStreamAsync(cancellationToken).Result;
      var json = await JsonSerializer.DeserializeAsync<ValidationResponse>(response.Content.ReadAsStreamAsync(cancellationToken).Result, (JsonSerializerOptions?)null, cancellationToken).ConfigureAwait(false);
      _logger.LogError("API returned response with status code {StatusCode} {Json}", response.StatusCode, json);
    }
    _logger.LogInformation("API returned response with status code {StatusCode}", response.StatusCode);
    var auth = await JsonSerializer.DeserializeAsync<Auth>(response.Content.ReadAsStreamAsync(cancellationToken).Result, (JsonSerializerOptions?)null, cancellationToken).ConfigureAwait(false) ?? throw new ApiException("Unexpected null return value.");
    return auth.Token;
  }
  private async Task<T> Get<T>(string url, IDictionary<string, string?>? query, string? token, CancellationToken cancellationToken)
  {
    var host = MediaDataHubUtils.GetHost();
    token ??= await AuthWithPassword(cancellationToken).ConfigureAwait(false);
    var remoteUrl = query != null ? QueryHelpers.AddQueryString(string.Concat(host, url), query) : string.Concat(host, url);
    using var requestMessage = new HttpRequestMessage(HttpMethod.Get, remoteUrl);
    requestMessage.Headers.Add("Authorization", token);
    using var response = await _httpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
    if (response.StatusCode != HttpStatusCode.OK)
    {

      var text = response.Content.ReadAsStreamAsync(cancellationToken).Result;
      var json = await JsonSerializer.DeserializeAsync<ValidationResponse>(response.Content.ReadAsStreamAsync(cancellationToken).Result, (JsonSerializerOptions?)null, cancellationToken).ConfigureAwait(false);
      _logger.LogError("API returned response with status code {StatusCode} {Json}", response.StatusCode, json);
      throw ApiException.FromResponse(response);
    }
    _logger.LogInformation("API returned response with status code {StatusCode}", response.StatusCode);
    return await JsonSerializer.DeserializeAsync<T>(response.Content.ReadAsStreamAsync(cancellationToken).Result, (JsonSerializerOptions?)null, cancellationToken).ConfigureAwait(false) ?? throw new ApiException("Unexpected null return value.");
  }

  public Task<Response<T>> List<T>(string collectionName, CancellationToken cancellationToken, IDictionary<string, string?>? query = null, string? token = null)
  {
    return Get<Response<T>>($"/api/collections/{collectionName}/records", query, token, cancellationToken);
  }

  public Task<T> View<T>(string collectionName, string id, CancellationToken cancellationToken, IDictionary<string, string?>? query = null, string? token = null)
  {
    return Get<T>($"/api/collections/{collectionName}/records/{id}", query, token, cancellationToken);
  }
}

using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaDataHub.Plugin.Api;
using MediaDataHub.Plugin.Api.Manager;
using Model = MediaDataHub.Plugin.Api.Model;

namespace MediaDataHub.Plugin.Provider.Movies;

public class MovieProvider : MediaDataHubProvider<Model.Movie, Movie, MovieInfo>
{
  private readonly MediaDataHubApiManager _apiManager;

  public MovieProvider(
    ILogger<MovieProvider> logger,
    MediaDataHubApiManager apiManager,
    IHttpClientFactory httpClientFactory
  ) : base(logger, httpClientFactory)
  {
    _apiManager = apiManager;
  }

  /// <inheritdoc />
  protected override Task<Model.Movie> GetById(string id, CancellationToken cancellationToken)
  {
    return _apiManager.GetMovieById(id, cancellationToken);
  }

  /// <inheritdoc />
  protected async override Task<IMetadataResult<Movie, MovieInfo>> GetDetailById(string id, CancellationToken cancellationToken)
  {
    return await _apiManager.GetMovieDetailById(id, cancellationToken).ConfigureAwait(false);
  }

  /// <inheritdoc />
  protected override Task<IEnumerable<Model.Movie>> Search(MovieInfo info, CancellationToken cancellationToken)
  {
    return _apiManager.SearchMovies(info.Name, info.Year, cancellationToken);
  }

  public override IEnumerable<ImageType> GetSupportedImages(BaseItem item) => new[] { ImageType.Primary, ImageType.Backdrop, ImageType.Banner, ImageType.Logo, ImageType.Thumb };
}

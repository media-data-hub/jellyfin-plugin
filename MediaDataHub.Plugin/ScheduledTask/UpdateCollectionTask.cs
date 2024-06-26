using Microsoft.Extensions.Logging;
using Jellyfin.Data.Enums;
using MediaBrowser.Controller.Collections;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Tasks;
using MediaDataHub.Plugin.Api;
using MediaDataHub.Plugin.Api.Manager;
using Model = MediaDataHub.Plugin.Api.Model;
using MediaDataHub.Plugin.Configuration;
using System.Collections.ObjectModel;

namespace MediaDataHub.Plugin.ScheduledTask;

public class UpdateCollectionTask : IScheduledTask
{
  private readonly ILogger<UpdateCollectionTask> _logger;
  private readonly ILibraryManager _libraryManager;
  private readonly IProviderManager _providerManager;
  private readonly ICollectionManager _collectionManager;
  private readonly MediaDataHubApiManager _apiManager;
  private readonly IFileSystem _fileSystem;
  public string Name => "Update Collections";
  public string Key => "MediaDataHub.UpdateCollectionTask";
  public string Description => "Add Tv series and movies and albums to collections";
  public string Category => "Media Data Hub";

  public UpdateCollectionTask(
    ILibraryManager libraryManager,
    IProviderManager providerManager,
    ICollectionManager collectionManager,
    MediaDataHubApiManager apiManager,
    ILogger<UpdateCollectionTask> logger,
    IFileSystem fileSystem)
  {
    _libraryManager = libraryManager;
    _providerManager = providerManager;
    _collectionManager = collectionManager;
    _logger = logger;
    _apiManager = apiManager;
    _fileSystem = fileSystem;
  }
  public async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Start Scheduled Task ({name})", Name);
    var config = MediaDataHubUtils.GetConfiguration();
    if (!config.AutoAddToCollection)
    {
      _logger.LogInformation("End Scheduled Task ({name}) [Disabled]", Name);
      return;
    }
    var query = new InternalItemsQuery
    {
      IncludeItemTypes = [BaseItemKind.Movie, BaseItemKind.Series, BaseItemKind.MusicAlbum],
      IsVirtualItem = false,
      Recursive = true
    };
    var items = _libraryManager.GetItemList(query).Select((item, index) => (item, index));
    Dictionary<string, Model.Collection> collectionDict = [];
    Dictionary<string, List<BaseItem>> mappingDict = [];
    foreach (var (item, index) in items)
    {
      if (!item.TryGetProviderId(Plugin.ProviderId, out var id))
      {
        continue;
      }
      try
      {
        IEnumerable<Model.Collection> collections;
        switch (item)
        {
          case Movie movie:
            var movieDetail = await _apiManager.GetMovieDetailById(id, cancellationToken).ConfigureAwait(false);
            collections = movieDetail.Expand.Collections;
            break;
          case Series series:
            var tvSeriesDetail = await _apiManager.GetTvSeriesDetailById(id, cancellationToken).ConfigureAwait(false);
            collections = tvSeriesDetail.Expand.Collections;
            break;
          case MusicAlbum series:
            var musicAlbumDetail = await _apiManager.GetMusicAlbumDetailById(id, cancellationToken).ConfigureAwait(false);
            collections = musicAlbumDetail.Expand.Collections;
            break;
          default:
            continue;
        }
        foreach (var collection in collections)
        {
          collectionDict[collection.Id] = collection;
          var itemList = mappingDict.GetValueOrDefault(collection.Id, []);
          itemList.Add(item);
          mappingDict[collection.Id] = itemList;
        }
      }
      catch (Model.ApiException e)
      {
        _logger.LogWarning(e, "Failed to load ({id})", id);
        continue;
      }
    }
    foreach (var collectionEntry in collectionDict.Select((value, index) => new { index, value = value.Value }))
    {
      var index = collectionEntry.index;
      progress?.Report(100.0 * index / collectionDict.Count);
      var collection = collectionEntry.value;
      var boxSet = await GetOrCreateBoxSet(collection, config);
      if (boxSet != null)
      {
        var itemsToAdd = mappingDict[collection.Id]
                .Where(item => !boxSet.ContainsLinkedChildByItemId(item.Id))
                .Select(item => item.Id)
                .ToList();
        if (itemsToAdd.Count != 0)
        {
          _logger.LogInformation("Add to boxSet ({id}, {name})", collection.Id, itemsToAdd);
          await _collectionManager.AddToCollectionAsync(boxSet.Id, itemsToAdd).ConfigureAwait(false);
        }
        if (config.AutoRefreshCollection)
        {
          _logger.LogInformation("Refresh boxSet ({id}, {name})", collection.Id, collection.Name);
          var refreshOptions = new MetadataRefreshOptions(new DirectoryService(_fileSystem))
          {
            MetadataRefreshMode = MetadataRefreshMode.FullRefresh,
            ImageRefreshMode = MetadataRefreshMode.FullRefresh,
            ReplaceAllImages = false,
            ReplaceAllMetadata = true,
            ForceSave = true,
            IsAutomated = false
          };
          _providerManager.QueueRefresh(boxSet.Id, refreshOptions, RefreshPriority.Normal);

        }
      }
      else
      {
        _logger.LogWarning("Failed to get or create boxSet ({id}, {name})", collection.Id, collection.Name);
      }
    }
    progress?.Report(100);
    _logger.LogInformation("End Scheduled Task ({name})", Name);
  }

  private async Task<BoxSet?> GetOrCreateBoxSet(ICollectionCreation collection, PluginConfiguration config)
  {
    var query = new InternalItemsQuery
    {
      IncludeItemTypes = [BaseItemKind.BoxSet],
      CollapseBoxSetItems = false,
      Recursive = true,
      HasAnyProviderId = new Dictionary<string, string> { { Plugin.ProviderId, collection.Id } }
    };
    var boxSet = _libraryManager.GetItemList(query).FirstOrDefault() as BoxSet;
    if (boxSet != null)
    {
      _logger.LogInformation("Find boxSet ({id}, {name})", collection.Id, collection.Name);
      return boxSet;
    }
    if (config.AutoConnectCollection)
    {
      query = new InternalItemsQuery
      {
        IncludeItemTypes = [BaseItemKind.BoxSet],
        CollapseBoxSetItems = false,
        Recursive = true,
        Name = collection.Name
      };
      boxSet = _libraryManager.GetItemList(query).FirstOrDefault() as BoxSet;
    }
    if (boxSet != null)
    {
      _logger.LogInformation("Find boxSet ({id}, {name})", collection.Id, collection.Name);
      return boxSet;
    }
    _logger.LogInformation("Cannot find boxSet ({id}, {name})", collection.Id, collection.Name);
    return config.AutoCreateCollection ? await _collectionManager.CreateCollectionAsync(collection.ToCollectionCreationOptions()).ConfigureAwait(false) : null;
  }

  public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
  {
    // Run this task every 24 hours
    return [new TaskTriggerInfo { Type = TaskTriggerInfo.TriggerInterval, IntervalTicks = TimeSpan.FromHours(24).Ticks }];
  }
}

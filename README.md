# Media Data Hub Plugin for Jellyfin

## Features

- [x] Movie
  - [x] Metadata
  - [x] Image
  - [ ] External Id
  - [x] Merge / Split Versions
- [x] TV Series
  - [x] Metadata
  - [x] Image
  - [ ] External Id
- [x] TV Season
  - [x] Metadata
  - [x] Image
  - [ ] External Id
- [x] TV Episode
  - [x] Metadata
  - [x] Image
  - [ ] External Id
  - [x] Merge / Split Versions
- [x] Collection
  - [x] Metadata
  - [x] Image
  - [ ] External Id
- [x] Person
  - [x] Metadata
  - [x] Image
  - [ ] External Id

## Develop

### Build

```sh
dotnet publish MediaDataHubPlugin.sln /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary
```

Copy `MediaDataHub.Plugin/bin/Debug/net8.0/publish/MediaDataHub.Plugin.dll` to Jellyfin Plugin folder.

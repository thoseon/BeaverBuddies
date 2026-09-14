using Timberborn.FileSystem;
using Timberborn.MapEditorPersistenceUI;
using Timberborn.MapMetadataSystem;
using Timberborn.MapRepositorySystem;
using Timberborn.MapThumbnail;
using UnityEngine;

namespace Timberborn.SteamWorkshopMapUploadingUI;

internal class SteamWorkshopUploadableMapFactory
{
	private readonly MapDeserializer _mapDeserializer;

	private readonly MapMetadataSerializer _mapMetadataSerializer;

	private readonly FilenameValidator _filenameValidator;

	private readonly SteamWorkshopMapDataService _steamWorkshopMapDataService;

	private readonly MapThumbnailCache _mapThumbnailCache;

	private readonly IFileService _fileService;

	private readonly MapRepository _mapRepository;

	private readonly MapSaverLoader _mapSaverLoader;

	public SteamWorkshopUploadableMapFactory(MapDeserializer mapDeserializer, MapMetadataSerializer mapMetadataSerializer, FilenameValidator filenameValidator, SteamWorkshopMapDataService steamWorkshopMapDataService, MapThumbnailCache mapThumbnailCache, IFileService fileService, MapRepository mapRepository, MapSaverLoader mapSaverLoader)
	{
		_mapDeserializer = mapDeserializer;
		_mapMetadataSerializer = mapMetadataSerializer;
		_filenameValidator = filenameValidator;
		_steamWorkshopMapDataService = steamWorkshopMapDataService;
		_mapThumbnailCache = mapThumbnailCache;
		_fileService = fileService;
		_mapRepository = mapRepository;
		_mapSaverLoader = mapSaverLoader;
	}

	public SteamWorkshopUploadableMap Create(MapFileReference mapFileReference)
	{
		_mapThumbnailCache.Clear();
		MapMetadata mapMetadata = _mapDeserializer.ReadFromMapFileUnsafe(mapFileReference, _mapMetadataSerializer);
		Texture2D thumbnail = _mapThumbnailCache.GetThumbnail(mapFileReference);
		SteamWorkshopMapContent steamWorkshopMapContent = new SteamWorkshopMapContent(_fileService, _mapRepository, thumbnail, mapFileReference);
		return new SteamWorkshopUploadableMap(_steamWorkshopMapDataService, _filenameValidator, _mapSaverLoader, steamWorkshopMapContent, mapFileReference, mapMetadata);
	}
}

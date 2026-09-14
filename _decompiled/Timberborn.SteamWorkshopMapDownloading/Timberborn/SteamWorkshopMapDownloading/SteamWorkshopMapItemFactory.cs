using System.Collections.Generic;
using System.IO;
using System.Linq;
using Timberborn.AssetSystem;
using Timberborn.MapItemsUI;
using Timberborn.MapMetadataSystem;
using Timberborn.MapRepositorySystem;
using Timberborn.SingletonSystem;
using Timberborn.SteamWorkshopContent;
using UnityEngine;

namespace Timberborn.SteamWorkshopMapDownloading;

internal class SteamWorkshopMapItemFactory : ICustomMapItemFactory, ILoadableSingleton
{
	private static readonly string CloudIconPath = "UI/Images/Core/cloud-file-icon";

	private static readonly string SteamMapLocKey = "SteamWorkshop.SteamMapTooltip";

	private readonly MapRepository _mapRepository;

	private readonly MapDeserializer _mapDeserializer;

	private readonly MapMetadataSerializer _mapMetadataSerializer;

	private readonly SteamWorkshopContentProvider _steamWorkshopContentProvider;

	private readonly IAssetLoader _assetLoader;

	private MapIcon _steamMapIcon;

	public SteamWorkshopMapItemFactory(MapRepository mapRepository, MapDeserializer mapDeserializer, MapMetadataSerializer mapMetadataSerializer, SteamWorkshopContentProvider steamWorkshopContentProvider, IAssetLoader assetLoader)
	{
		_mapRepository = mapRepository;
		_mapDeserializer = mapDeserializer;
		_mapMetadataSerializer = mapMetadataSerializer;
		_steamWorkshopContentProvider = steamWorkshopContentProvider;
		_assetLoader = assetLoader;
	}

	public void Load()
	{
		_steamMapIcon = new MapIcon(_assetLoader.Load<Sprite>(CloudIconPath), SteamMapLocKey);
	}

	public IEnumerable<MapItem> Create()
	{
		return from item in CreateInternal()
			orderby item.DisplayName
			select item;
	}

	private IEnumerable<MapItem> CreateInternal()
	{
		foreach (DirectoryInfo contentDirectory in _steamWorkshopContentProvider.GetContentDirectories())
		{
			foreach (FileInfo item in _mapRepository.GetMapFilesFromDirectory(contentDirectory))
			{
				yield return Create(item);
			}
		}
	}

	private MapItem Create(FileInfo mapFile)
	{
		MapFileReference mapFileReference = MapFileReference.FromDisk(mapFile.FullName);
		MapMetadata mapMetadata = _mapDeserializer.ReadFromMapFile(mapFileReference, _mapMetadataSerializer);
		return new MapItem(mapFileReference, mapFileReference.Name, mapMetadata?.MapDescription, GetSize(mapMetadata), isRecommended: false, isUnconventional: false, isDeletable: false, isDev: false, _steamMapIcon);
	}

	private static Vector2Int? GetSize(MapMetadata mapMetadata)
	{
		if (mapMetadata != null)
		{
			return new Vector2Int(mapMetadata.Width, mapMetadata.Height);
		}
		return null;
	}
}

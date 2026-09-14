using System.Collections.Generic;
using System.Linq;
using Timberborn.AssetSystem;
using Timberborn.MapMetadataSystem;
using Timberborn.MapRepositorySystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.MapItemsUI;

public class UserMapItemFactory : ILoadableSingleton
{
	private static readonly string LocalMapLocKey = "MapSelection.LocalMap";

	private readonly MapDeserializer _mapDeserializer;

	private readonly MapMetadataSerializer _mapMetadataSerializer;

	private readonly MapRepository _mapRepository;

	private readonly IAssetLoader _assetLoader;

	private MapIcon _userMapIcon;

	public UserMapItemFactory(MapDeserializer mapDeserializer, MapMetadataSerializer mapMetadataSerializer, MapRepository mapRepository, IAssetLoader assetLoader)
	{
		_mapDeserializer = mapDeserializer;
		_mapMetadataSerializer = mapMetadataSerializer;
		_mapRepository = mapRepository;
		_assetLoader = assetLoader;
	}

	public void Load()
	{
		_userMapIcon = new MapIcon(_assetLoader.Load<Sprite>("UI/Images/Core/local-file-icon"), LocalMapLocKey);
	}

	public IEnumerable<MapItem> Create()
	{
		return _mapRepository.GetUserMapNames().Select(Create);
	}

	private MapItem Create(string name)
	{
		MapFileReference mapFileReference = MapFileReference.FromUserFolder(name);
		MapMetadata mapMetadata = _mapDeserializer.ReadFromMapFile(mapFileReference, _mapMetadataSerializer);
		return new MapItem(mapFileReference, mapFileReference.Name, mapMetadata?.MapDescription, GetSize(mapMetadata), isRecommended: false, isUnconventional: false, isDeletable: true, isDev: false, _userMapIcon);
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

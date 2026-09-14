using System.Collections.Generic;
using System.Linq;
using Timberborn.AssetSystem;
using Timberborn.Debugging;
using Timberborn.Localization;
using Timberborn.MapMetadataSystem;
using Timberborn.MapRepositorySystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.MapItemsUI;

public class OfficialMapItemFactory : ILoadableSingleton
{
	private readonly ILoc _loc;

	private readonly MapDeserializer _mapDeserializer;

	private readonly MapMetadataSerializer _mapMetadataSerializer;

	private readonly MapRepository _mapRepository;

	private readonly DevModeManager _devModeManager;

	private readonly IAssetLoader _assetLoader;

	private MapIcon _devIcon;

	public OfficialMapItemFactory(ILoc loc, MapDeserializer mapDeserializer, MapMetadataSerializer mapMetadataSerializer, MapRepository mapRepository, DevModeManager devModeManager, IAssetLoader assetLoader)
	{
		_loc = loc;
		_mapDeserializer = mapDeserializer;
		_mapMetadataSerializer = mapMetadataSerializer;
		_mapRepository = mapRepository;
		_devModeManager = devModeManager;
		_assetLoader = assetLoader;
	}

	public void Load()
	{
		_devIcon = new MapIcon(_assetLoader.Load<Sprite>("UI/Images/Core/dev-map-icon"), null);
	}

	public IEnumerable<MapItem> Create()
	{
		return from mapItem in _mapRepository.GetBuiltinMapNames().Select(Create)
			where !mapItem.IsDev || _devModeManager.Enabled
			orderby mapItem.IsDev, !mapItem.IsRecommended, mapItem.IsUnconventional, mapItem.DisplayName
			select mapItem;
	}

	private MapItem Create(string name)
	{
		MapFileReference mapFileReference = MapFileReference.FromResource(name);
		MapMetadata mapMetadata = _mapDeserializer.ReadFromMapFile(mapFileReference, _mapMetadataSerializer);
		return new MapItem(mapFileReference, GetDisplayName(mapFileReference, mapMetadata), GetDisplayDescription(mapMetadata), new Vector2Int(mapMetadata.Width, mapMetadata.Height), mapMetadata.IsRecommended, mapMetadata.IsUnconventional, isDeletable: false, mapMetadata.IsDev, mapMetadata.IsDev ? _devIcon : null);
	}

	private string GetDisplayName(MapFileReference mapFileReference, MapMetadata mapMetadata)
	{
		if (!string.IsNullOrEmpty(mapMetadata.MapNameLocKey))
		{
			return _loc.T(mapMetadata.MapNameLocKey);
		}
		return mapFileReference.Name;
	}

	private string GetDisplayDescription(MapMetadata mapMetadata)
	{
		if (!string.IsNullOrEmpty(mapMetadata.MapDescriptionLocKey))
		{
			return _loc.T(mapMetadata.MapDescriptionLocKey);
		}
		return null;
	}
}

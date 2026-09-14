using Timberborn.Common;
using Timberborn.MapEditorPersistence;
using Timberborn.MapRepositorySystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.SteamWorkshop;
using Timberborn.WorldPersistence;

namespace Timberborn.SteamWorkshopMapUploadingUI;

public class SteamWorkshopMapDataService : ISaveableSingleton, ILoadableSingleton
{
	private static readonly SingletonKey SteamWorkshopMapDataServiceKey = new SingletonKey("SteamWorkshopMapDataService");

	private static readonly PropertyKey<SteamWorkshopItem> SteamWorkshopItemKey = new PropertyKey<SteamWorkshopItem>("SteamWorkshopItem");

	private readonly MapEditorMapLoader _mapEditorMapLoader;

	private readonly SteamWorkshopItemSerializer _steamWorkshopItemSerializer;

	private readonly ISingletonLoader _singletonLoader;

	public SteamWorkshopItem SteamWorkshopItem { get; private set; }

	public SteamWorkshopMapDataService(MapEditorMapLoader mapEditorMapLoader, SteamWorkshopItemSerializer steamWorkshopItemSerializer, ISingletonLoader singletonLoader)
	{
		_mapEditorMapLoader = mapEditorMapLoader;
		_steamWorkshopItemSerializer = steamWorkshopItemSerializer;
		_singletonLoader = singletonLoader;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (SteamWorkshopItem != null)
		{
			singletonSaver.GetSingleton(SteamWorkshopMapDataServiceKey).Set(SteamWorkshopItemKey, SteamWorkshopItem, _steamWorkshopItemSerializer);
		}
	}

	[BackwardCompatible(2024, 5, 7, Compatibility.Map)]
	public void Load()
	{
		MapFileReference? loadedMap = _mapEditorMapLoader.LoadedMap;
		if (!loadedMap.HasValue || !loadedMap.GetValueOrDefault().UserFolder || !_singletonLoader.TryGetSingleton(SteamWorkshopMapDataServiceKey, out var objectLoader))
		{
			return;
		}
		if (objectLoader.Has(SteamWorkshopItemKey))
		{
			SteamWorkshopItem = objectLoader.Get(SteamWorkshopItemKey, _steamWorkshopItemSerializer);
			return;
		}
		PropertyKey<SteamWorkshopItem> key = new PropertyKey<SteamWorkshopItem>("SteamWorkshopItemData");
		if (objectLoader.Has(key))
		{
			SteamWorkshopItem = objectLoader.Get(key, _steamWorkshopItemSerializer);
		}
	}

	public void SetMapData(SteamWorkshopItem item)
	{
		SteamWorkshopItem = item;
	}
}

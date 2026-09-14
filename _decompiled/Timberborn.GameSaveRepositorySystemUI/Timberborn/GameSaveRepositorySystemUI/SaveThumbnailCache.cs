using Timberborn.GameSaveRepositorySystem;
using Timberborn.SaveThumbnail;
using Timberborn.SingletonSystem;
using Timberborn.ThumbnailSystem;
using UnityEngine;

namespace Timberborn.GameSaveRepositorySystemUI;

public class SaveThumbnailCache : ILoadableSingleton, IUnloadableSingleton
{
	private readonly GameSaveDeserializer _gameSaveDeserializer;

	private readonly SaveThumbnailSaveEntryReader _saveThumbnailSaveEntryReader;

	private ThumbnailCache<SaveReference> _thumbnailCache;

	public SaveThumbnailCache(GameSaveDeserializer gameSaveDeserializer, SaveThumbnailSaveEntryReader saveThumbnailSaveEntryReader)
	{
		_gameSaveDeserializer = gameSaveDeserializer;
		_saveThumbnailSaveEntryReader = saveThumbnailSaveEntryReader;
	}

	public void Load()
	{
		_thumbnailCache = new ThumbnailCache<SaveReference>((SaveReference save) => _gameSaveDeserializer.ReadFromSaveFile(save, _saveThumbnailSaveEntryReader));
	}

	public void Unload()
	{
		Clear();
	}

	public void Clear()
	{
		_thumbnailCache.Clear();
	}

	public Texture2D GetThumbnail(SaveReference saveReference)
	{
		return _thumbnailCache.GetThumbnail(saveReference);
	}
}

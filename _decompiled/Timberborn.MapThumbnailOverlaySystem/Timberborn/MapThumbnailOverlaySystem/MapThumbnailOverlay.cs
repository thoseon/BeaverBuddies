using System.IO;
using Timberborn.MapEditorPersistence;
using Timberborn.MapRepositorySystem;
using Timberborn.MapThumbnail;
using Timberborn.SingletonSystem;
using Timberborn.ThumbnailCapturing;
using Timberborn.UndoSystem;
using UnityEngine;

namespace Timberborn.MapThumbnailOverlaySystem;

public class MapThumbnailOverlay : ILoadableSingleton, IUnloadableSingleton
{
	private class ThumbnailOverlayUndoable : IUndoable
	{
		private readonly MapThumbnailOverlay _mapThumbnailOverlay;

		private readonly byte[] _oldOverlay;

		private readonly byte[] _newOverlay;

		public ThumbnailOverlayUndoable(MapThumbnailOverlay mapThumbnailOverlay, byte[] oldOverlay, byte[] newOverlay)
		{
			_mapThumbnailOverlay = mapThumbnailOverlay;
			_oldOverlay = oldOverlay;
			_newOverlay = newOverlay;
		}

		public void Undo()
		{
			if (_oldOverlay == null)
			{
				_mapThumbnailOverlay.ClearInternal(registerUndo: false);
			}
			else
			{
				_mapThumbnailOverlay.LoadFromBytes(_oldOverlay, registerUndo: false);
			}
			_mapThumbnailOverlay.NotifyThumbnailChanged();
		}

		public void Redo()
		{
			if (_newOverlay == null)
			{
				_mapThumbnailOverlay.ClearInternal(registerUndo: false);
			}
			else
			{
				_mapThumbnailOverlay.LoadFromBytes(_newOverlay, registerUndo: false);
			}
			_mapThumbnailOverlay.NotifyThumbnailChanged();
		}
	}

	private readonly MapThumbnailConfiguration _mapThumbnailConfiguration;

	private readonly IThumbnailRenderTextureProvider _thumbnailRenderTextureProvider;

	private readonly MapEditorMapLoader _mapEditorMapLoader;

	private readonly MapDeserializer _mapDeserializer;

	private readonly MapThumbnailOverlaySerializer _mapThumbnailOverlaySerializer;

	private readonly IUndoRegistry _undoRegistry;

	private readonly EventBus _eventBus;

	public Texture2D Overlay { get; private set; }

	public MapThumbnailOverlay(MapThumbnailConfiguration mapThumbnailConfiguration, IThumbnailRenderTextureProvider thumbnailRenderTextureProvider, MapEditorMapLoader mapEditorMapLoader, MapDeserializer mapDeserializer, MapThumbnailOverlaySerializer mapThumbnailOverlaySerializer, IUndoRegistry undoRegistry, EventBus eventBus)
	{
		_mapThumbnailConfiguration = mapThumbnailConfiguration;
		_thumbnailRenderTextureProvider = thumbnailRenderTextureProvider;
		_mapEditorMapLoader = mapEditorMapLoader;
		_mapDeserializer = mapDeserializer;
		_mapThumbnailOverlaySerializer = mapThumbnailOverlaySerializer;
		_undoRegistry = undoRegistry;
		_eventBus = eventBus;
	}

	public void Load()
	{
		if (_mapEditorMapLoader.LoadedMap.HasValue)
		{
			MapFileReference value = _mapEditorMapLoader.LoadedMap.Value;
			byte[] array = _mapDeserializer.ReadFromMapFile(value, _mapThumbnailOverlaySerializer);
			if (array != null && array.Length > 0)
			{
				LoadFromBytes(array, registerUndo: false);
			}
		}
	}

	public void Unload()
	{
		ClearInternal(registerUndo: false);
	}

	public void LoadFromFile(string path)
	{
		byte[] fileData = File.ReadAllBytes(path);
		LoadFromBytes(fileData, registerUndo: true);
	}

	public void Clear()
	{
		ClearInternal(registerUndo: true);
	}

	private void LoadFromBytes(byte[] fileData, bool registerUndo)
	{
		byte[] oldOverlay = Overlay?.EncodeToPNG();
		ClearInternal(registerUndo: false);
		Overlay = new Texture2D(1, 1, TextureFormat.ARGB32, mipChain: false)
		{
			filterMode = FilterMode.Bilinear
		};
		if (Overlay.LoadImage(fileData))
		{
			Resize();
			if (registerUndo)
			{
				_undoRegistry.RegisterSingleUndoable(new ThumbnailOverlayUndoable(this, oldOverlay, Overlay.EncodeToPNG()));
			}
		}
		else
		{
			ClearInternal(registerUndo);
		}
	}

	private void ClearInternal(bool registerUndo)
	{
		if ((bool)Overlay)
		{
			if (registerUndo)
			{
				_undoRegistry.RegisterSingleUndoable(new ThumbnailOverlayUndoable(this, Overlay.EncodeToPNG(), null));
			}
			Object.Destroy(Overlay);
			Overlay = null;
		}
	}

	private void Resize()
	{
		(int, int) scaledOverlaySize = GetScaledOverlaySize();
		int item = scaledOverlaySize.Item1;
		int item2 = scaledOverlaySize.Item2;
		RenderTextureFormat format = _thumbnailRenderTextureProvider.RenderTexture.format;
		RenderTexture renderTexture = (RenderTexture.active = RenderTexture.GetTemporary(item, item2, 0, format, RenderTextureReadWrite.Default));
		Graphics.Blit(Overlay, renderTexture);
		Overlay.Reinitialize(item, item2, TextureFormat.ARGB32, hasMipMap: false);
		Overlay.ReadPixels(new Rect(Vector2.zero, new Vector2(item, item2)), 0, 0);
		Overlay.Apply();
		RenderTexture.active = null;
		RenderTexture.ReleaseTemporary(renderTexture);
	}

	private (int, int) GetScaledOverlaySize()
	{
		int width = _mapThumbnailConfiguration.Width;
		int height = _mapThumbnailConfiguration.Height;
		int width2 = Overlay.width;
		int height2 = Overlay.height;
		float num = Mathf.Min((float)width / (float)width2, (float)height / (float)height2);
		return ((int)((float)width2 * num), (int)((float)height2 * num));
	}

	private void NotifyThumbnailChanged()
	{
		_eventBus.Post(new MapThumbnailChangedEvent());
	}
}

using Timberborn.MapThumbnail;
using Timberborn.SingletonSystem;
using Timberborn.ThumbnailCapturing;
using UnityEngine;

namespace Timberborn.MapThumbnailCapturing;

public class MapThumbnailRenderTextureProvider : IThumbnailRenderTextureProvider, ILoadableSingleton, IUnloadableSingleton
{
	private readonly MapThumbnailConfiguration _mapThumbnailConfiguration;

	public RenderTexture RenderTexture { get; private set; }

	public MapThumbnailRenderTextureProvider(MapThumbnailConfiguration mapThumbnailConfiguration)
	{
		_mapThumbnailConfiguration = mapThumbnailConfiguration;
	}

	public void Load()
	{
		RenderTexture = new RenderTexture(_mapThumbnailConfiguration.Width, _mapThumbnailConfiguration.Height, 1, RenderTextureFormat.ARGB32, 0);
	}

	public void Unload()
	{
		RenderTexture.Release();
		Object.Destroy(RenderTexture);
	}
}

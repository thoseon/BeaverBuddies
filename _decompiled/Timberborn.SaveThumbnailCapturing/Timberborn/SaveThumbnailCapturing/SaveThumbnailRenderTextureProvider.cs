using Timberborn.SaveThumbnail;
using Timberborn.SingletonSystem;
using Timberborn.ThumbnailCapturing;
using UnityEngine;

namespace Timberborn.SaveThumbnailCapturing;

public class SaveThumbnailRenderTextureProvider : IThumbnailRenderTextureProvider, ILoadableSingleton, IUnloadableSingleton
{
	private readonly SaveThumbnailConfiguration _saveThumbnailConfiguration;

	public RenderTexture RenderTexture { get; private set; }

	public SaveThumbnailRenderTextureProvider(SaveThumbnailConfiguration saveThumbnailConfiguration)
	{
		_saveThumbnailConfiguration = saveThumbnailConfiguration;
	}

	public void Load()
	{
		RenderTexture = new RenderTexture(_saveThumbnailConfiguration.Width, _saveThumbnailConfiguration.Height, 1, RenderTextureFormat.ARGB32, 0);
	}

	public void Unload()
	{
		if ((bool)RenderTexture)
		{
			RenderTexture.Release();
			Object.Destroy(RenderTexture);
		}
	}
}

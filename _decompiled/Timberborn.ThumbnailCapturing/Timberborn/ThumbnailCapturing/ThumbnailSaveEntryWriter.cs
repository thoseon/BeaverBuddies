using System.IO;
using Timberborn.TextureOperations;
using Timberborn.ThumbnailSystem;
using UnityEngine;

namespace Timberborn.ThumbnailCapturing;

public class ThumbnailSaveEntryWriter
{
	private readonly ThumbnailRenderer _thumbnailRenderer;

	private readonly IThumbnailRenderTextureProvider _thumbnailRenderTextureProvider;

	private readonly ThumbnailSerializer _thumbnailSerializer;

	private readonly TextureFactory _textureFactory;

	public ThumbnailSaveEntryWriter(ThumbnailRenderer thumbnailRenderer, IThumbnailRenderTextureProvider thumbnailRenderTextureProvider, ThumbnailSerializer thumbnailSerializer, TextureFactory textureFactory)
	{
		_thumbnailRenderer = thumbnailRenderer;
		_thumbnailRenderTextureProvider = thumbnailRenderTextureProvider;
		_thumbnailSerializer = thumbnailSerializer;
		_textureFactory = textureFactory;
	}

	public void WriteToSaveEntryStream(Stream entryStream, IThumbnailConfiguration thumbnailConfiguration, Texture2D overlay = null)
	{
		if ((bool)_thumbnailRenderTextureProvider.RenderTexture)
		{
			_thumbnailRenderer.Render();
			TextureSettings textureSettings = new TextureSettings.Builder().SetSize(thumbnailConfiguration.Width, thumbnailConfiguration.Height).SetTextureFormat(thumbnailConfiguration.TextureFormat).SetGenerateMipmap(generateMipmap: false)
				.Build();
			RenderTexture renderTexture = _thumbnailRenderTextureProvider.RenderTexture;
			Texture2D texture2D = _textureFactory.CreateTexture(textureSettings, renderTexture);
			if ((bool)overlay)
			{
				AddOverlay(texture2D, overlay);
			}
			texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: false);
			_thumbnailSerializer.WriteToSaveEntryStream(entryStream, texture2D, thumbnailConfiguration);
			Object.Destroy(texture2D);
		}
	}

	private static void AddOverlay(Texture2D thumbnail, Texture2D overlay)
	{
		int num = (thumbnail.width - overlay.width) / 2;
		int num2 = (thumbnail.height - overlay.height) / 2;
		for (int i = 0; i < overlay.width; i++)
		{
			for (int j = 0; j < overlay.height; j++)
			{
				Color pixel = thumbnail.GetPixel(i + num, j + num2);
				Color pixel2 = overlay.GetPixel(i, j);
				Color color = Color.Lerp(pixel, pixel2, pixel2.a / 1f);
				thumbnail.SetPixel(i + num, j + num2, color);
			}
		}
	}
}

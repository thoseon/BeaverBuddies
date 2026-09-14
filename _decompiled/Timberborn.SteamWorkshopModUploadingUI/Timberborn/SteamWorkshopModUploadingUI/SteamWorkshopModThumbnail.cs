using System.IO;
using Timberborn.TextureOperations;
using UnityEngine;

namespace Timberborn.SteamWorkshopModUploadingUI;

internal class SteamWorkshopModThumbnail
{
	private static readonly string[] PreviewNames = new string[3] { "thumbnail.png", "thumbnail.jpg", "thumbnail.jpeg" };

	private static readonly string DefaultPreviewAssetPath = Path.Combine("Modding", "mod-thumbnail.png");

	private readonly TextureFactory _textureFactory;

	private readonly string _modPath;

	public Texture2D Thumbnail { get; private set; }

	public SteamWorkshopModThumbnail(TextureFactory textureFactory, string modPath)
	{
		_textureFactory = textureFactory;
		_modPath = modPath;
	}

	public void UpdateThumbnail()
	{
		Clear();
		if (TryGetCustomThumbnailPath(out var previewPath))
		{
			TextureSettings textureSettings = new TextureSettings.Builder().SetSpritePreset().Build();
			if (_textureFactory.TryCreateTexture(textureSettings, File.ReadAllBytes(previewPath), out var texture))
			{
				Thumbnail = texture;
			}
		}
	}

	public void Clear()
	{
		if ((bool)Thumbnail)
		{
			Object.Destroy(Thumbnail);
			Thumbnail = null;
		}
	}

	public string GetThumbnailPath()
	{
		if (!TryGetCustomThumbnailPath(out var previewPath))
		{
			return Path.Combine(Application.streamingAssetsPath, DefaultPreviewAssetPath);
		}
		return previewPath;
	}

	private bool TryGetCustomThumbnailPath(out string previewPath)
	{
		string[] previewNames = PreviewNames;
		foreach (string path in previewNames)
		{
			previewPath = Path.Combine(_modPath, path);
			if (File.Exists(previewPath))
			{
				return true;
			}
		}
		previewPath = null;
		return false;
	}
}

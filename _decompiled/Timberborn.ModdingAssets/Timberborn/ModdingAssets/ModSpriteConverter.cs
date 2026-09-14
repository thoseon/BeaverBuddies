using System.Collections.Generic;
using System.IO;
using Timberborn.SerializationSystem;
using Timberborn.TextureOperations;
using UnityEngine;

namespace Timberborn.ModdingAssets;

internal class ModSpriteConverter : IModFileConverter<Sprite>
{
	private static readonly List<string> ValidExtensions = new List<string> { ".png", ".jpg" };

	private readonly TextureFactory _textureFactory;

	private readonly ModTextureSettingLoader _modTextureSettingLoader;

	private readonly List<Texture2D> _textures = new List<Texture2D>();

	private readonly List<Sprite> _sprites = new List<Sprite>();

	public ModSpriteConverter(TextureFactory textureFactory, ModTextureSettingLoader modTextureSettingLoader)
	{
		_textureFactory = textureFactory;
		_modTextureSettingLoader = modTextureSettingLoader;
	}

	public bool CanConvert(FileInfo fileInfo)
	{
		return ValidExtensions.Contains(fileInfo.Extension);
	}

	public bool TryConvert(OrderedFile orderedFile, string path, SerializedObject metadata, out Sprite asset)
	{
		FileInfo file = orderedFile.File;
		TextureSettings textureSettings = _modTextureSettingLoader.Load(file, metadata);
		if (_textureFactory.TryCreateTexture(textureSettings, File.ReadAllBytes(file.FullName), out var texture))
		{
			asset = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			_textures.Add(texture);
			_sprites.Add(asset);
			return true;
		}
		asset = null;
		return false;
	}

	public void Reset()
	{
		foreach (Sprite sprite in _sprites)
		{
			Object.Destroy(sprite);
		}
		foreach (Texture2D texture in _textures)
		{
			Object.Destroy(texture);
		}
		_textures.Clear();
		_sprites.Clear();
	}
}

using System.Collections.Generic;
using System.IO;
using Timberborn.SerializationSystem;
using Timberborn.TextureOperations;
using UnityEngine;

namespace Timberborn.ModdingAssets;

internal class ModTextureConverter : IModFileConverter<Texture2D>
{
	private static readonly List<string> ValidExtensions = new List<string> { ".png", ".jpg" };

	private readonly TextureFactory _textureFactory;

	private readonly ModTextureSettingLoader _modTextureSettingLoader;

	private readonly List<Texture2D> _textures = new List<Texture2D>();

	public ModTextureConverter(TextureFactory textureFactory, ModTextureSettingLoader modTextureSettingLoader)
	{
		_textureFactory = textureFactory;
		_modTextureSettingLoader = modTextureSettingLoader;
	}

	public bool CanConvert(FileInfo fileInfo)
	{
		return ValidExtensions.Contains(fileInfo.Extension);
	}

	public bool TryConvert(OrderedFile orderedFile, string path, SerializedObject metadata, out Texture2D asset)
	{
		FileInfo file = orderedFile.File;
		TextureSettings textureSettings = _modTextureSettingLoader.Load(file, metadata);
		if (_textureFactory.TryCreateTexture(textureSettings, File.ReadAllBytes(file.FullName), out asset))
		{
			_textures.Add(asset);
			return true;
		}
		return false;
	}

	public void Reset()
	{
		foreach (Texture2D texture in _textures)
		{
			Object.Destroy(texture);
		}
		_textures.Clear();
	}
}

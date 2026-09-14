using System.Collections.Generic;
using System.IO;
using System.Linq;
using Timberborn.FileSystem;
using Timberborn.PlatformUtilities;
using Timberborn.SingletonSystem;
using Timberborn.TextureOperations;
using UnityEngine;

namespace Timberborn.DecalSystem;

public class UserDecalTextureRepository : IUnloadableSingleton
{
	private static readonly string[] ValidExtensions = new string[3] { ".png", ".jpg", ".jpeg" };

	private readonly IFileService _fileService;

	private readonly TextureFactory _textureFactory;

	private readonly Dictionary<string, List<Texture2D>> _loadedTextures = new Dictionary<string, List<Texture2D>>();

	public UserDecalTextureRepository(IFileService fileService, TextureFactory textureFactory)
	{
		_fileService = fileService;
		_textureFactory = textureFactory;
	}

	public void Unload()
	{
		UnloadAllTextures();
	}

	public IEnumerable<Texture2D> LoadCustomTextures(string category)
	{
		if (!_loadedTextures.ContainsKey(category))
		{
			_loadedTextures[category] = new List<Texture2D>();
		}
		string text = Path.Combine(UserDataFolder.Folder, category);
		_fileService.CreateDirectory(text);
		IEnumerable<string> paths = from path in Directory.GetFiles(text)
			where ValidExtensions.Contains(Path.GetExtension(path))
			select path;
		UnloadTextures(category);
		LoadTextures(category, paths);
		return _loadedTextures[category];
	}

	public string GetCustomDecalDirectory(string category)
	{
		return Path.Combine(UserDataFolder.Folder, category);
	}

	private void LoadTextures(string category, IEnumerable<string> paths)
	{
		foreach (string path in paths)
		{
			try
			{
				byte[] bytes = File.ReadAllBytes(path);
				TextureSettings textureSettings = new TextureSettings.Builder().SetFilterMode(FilterMode.Bilinear).SetName(Path.GetFileName(path)).Build();
				if (_textureFactory.TryCreateTexture(textureSettings, bytes, out var texture))
				{
					_loadedTextures[category].Add(texture);
				}
			}
			catch (IOException)
			{
				Debug.LogError("Failed to load tail texture from path: " + path);
			}
		}
	}

	private void UnloadAllTextures()
	{
		foreach (string key in _loadedTextures.Keys)
		{
			UnloadTextures(key);
		}
		_loadedTextures.Clear();
	}

	private void UnloadTextures(string category)
	{
		if (!_loadedTextures.TryGetValue(category, out var value))
		{
			return;
		}
		foreach (Texture2D item in value)
		{
			Object.Destroy(item);
		}
		value.Clear();
	}
}

using System.IO;
using Timberborn.AssetSystem;
using UnityEngine;

namespace Timberborn.StatusSystem;

public class StatusSpriteLoader
{
	private static readonly string StatusSpriteDirectory = "Sprites/StatusIcons";

	private readonly IAssetLoader _assetLoader;

	public StatusSpriteLoader(IAssetLoader assetLoader)
	{
		_assetLoader = assetLoader;
	}

	public Sprite LoadSprite(string spriteName)
	{
		string path = Path.Combine(StatusSpriteDirectory, spriteName);
		return _assetLoader.Load<Sprite>(path);
	}
}

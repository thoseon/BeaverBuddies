using System.IO;
using Timberborn.AssetSystem;
using Timberborn.PrioritySystem;
using Timberborn.PrioritySystemUI;
using UnityEngine;

namespace Timberborn.BuilderPrioritySystemUI;

public class BuilderPrioritySpriteLoader : IPrioritySpriteLoader
{
	private static readonly string PrioritySpriteDirectory = "Sprites/Priority";

	private static readonly string PanelDirectory = "Panel";

	private static readonly string ButtonsDirectory = "Buttons";

	private readonly IAssetLoader _assetLoader;

	public BuilderPrioritySpriteLoader(IAssetLoader assetLoader)
	{
		_assetLoader = assetLoader;
	}

	public Sprite LoadSprite(Priority priority)
	{
		return LoadSprite(priority, PanelDirectory);
	}

	public Sprite LoadButtonSprite(Priority priority)
	{
		return LoadSprite(priority, ButtonsDirectory);
	}

	private Sprite LoadSprite(Priority priority, string subfolder)
	{
		string path = Path.Combine(PrioritySpriteDirectory, subfolder, priority.ToString());
		return _assetLoader.Load<Sprite>(path);
	}
}

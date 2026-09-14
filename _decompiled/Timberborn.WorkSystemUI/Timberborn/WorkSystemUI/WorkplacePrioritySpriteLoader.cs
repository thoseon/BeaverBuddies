using System.IO;
using Timberborn.AssetSystem;
using Timberborn.PrioritySystem;
using Timberborn.PrioritySystemUI;
using UnityEngine;

namespace Timberborn.WorkSystemUI;

public class WorkplacePrioritySpriteLoader : IPrioritySpriteLoader
{
	private static readonly string PrioritySpriteDirectory = "Sprites/Priority/Workplace";

	private readonly IAssetLoader _assetLoader;

	public WorkplacePrioritySpriteLoader(IAssetLoader assetLoader)
	{
		_assetLoader = assetLoader;
	}

	public Sprite LoadSprite(Priority priority)
	{
		string path = Path.Combine(PrioritySpriteDirectory, priority.ToString());
		return _assetLoader.Load<Sprite>(path);
	}
}

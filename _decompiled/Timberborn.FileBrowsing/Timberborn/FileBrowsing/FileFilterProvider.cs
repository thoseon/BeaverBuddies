using Timberborn.AssetSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.FileBrowsing;

public class FileFilterProvider : ILoadableSingleton
{
	private readonly IAssetLoader _assetLoader;

	public FileFilter Images { get; private set; }

	public FileFilterProvider(IAssetLoader assetLoader)
	{
		_assetLoader = assetLoader;
	}

	public void Load()
	{
		Images = new FileFilter(_assetLoader.Load<Sprite>("UI/Images/Core/image-icon"), new string[2] { ".png", ".jpg" });
	}
}

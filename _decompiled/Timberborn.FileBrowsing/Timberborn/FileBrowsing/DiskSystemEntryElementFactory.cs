using Timberborn.AssetSystem;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.FileBrowsing;

public class DiskSystemEntryElementFactory : ILoadableSingleton
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly IAssetLoader _assetLoader;

	private Sprite _directoryIcon;

	public DiskSystemEntryElementFactory(VisualElementLoader visualElementLoader, IAssetLoader assetLoader)
	{
		_visualElementLoader = visualElementLoader;
		_assetLoader = assetLoader;
	}

	public void Load()
	{
		_directoryIcon = _assetLoader.Load<Sprite>("UI/Images/Core/directory-icon");
	}

	public VisualElement Create(EventCallback<ClickEvent> onClick)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/DiskSystemEntryElement");
		visualElement.RegisterCallback(onClick);
		return visualElement;
	}

	public void Bind(VisualElement item, DiskSystemEntry diskSystemEntry, FileFilter fileFilter)
	{
		item.Q<Label>("Name").text = diskSystemEntry.Name;
		item.Q<Image>("Icon").sprite = (diskSystemEntry.IsDirectory ? _directoryIcon : fileFilter.Icon);
	}
}

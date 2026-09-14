using Timberborn.AssetSystem;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class VisualElementLoader
{
	private static readonly string ViewsDirectory = "UI/Views";

	private readonly IAssetLoader _assetLoader;

	private readonly VisualElementInitializer _visualElementInitializer;

	public VisualElementLoader(IAssetLoader assetLoader, VisualElementInitializer visualElementInitializer)
	{
		_assetLoader = assetLoader;
		_visualElementInitializer = visualElementInitializer;
	}

	public VisualElement LoadVisualElement(string elementName)
	{
		return LoadVisualElement(LoadVisualTreeAsset(elementName));
	}

	public VisualTreeAsset LoadVisualTreeAsset(string elementName)
	{
		string path = ViewsDirectory + "/" + elementName;
		return _assetLoader.Load<VisualTreeAsset>(path);
	}

	private VisualElement LoadVisualElement(VisualTreeAsset visualTreeAsset)
	{
		VisualElement visualElement = visualTreeAsset.CloneTree().ElementAt(0);
		_visualElementInitializer.InitializeVisualElement(visualElement);
		return visualElement;
	}
}

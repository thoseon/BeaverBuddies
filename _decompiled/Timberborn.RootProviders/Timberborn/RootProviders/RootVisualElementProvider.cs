using Timberborn.AssetSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.RootProviders;

public class RootVisualElementProvider
{
	private readonly IAssetLoader _assetLoader;

	private readonly RootObjectProvider _rootObjectProvider;

	public RootVisualElementProvider(IAssetLoader assetLoader, RootObjectProvider rootObjectProvider)
	{
		_assetLoader = assetLoader;
		_rootObjectProvider = rootObjectProvider;
	}

	public VisualElement Create(GameObject parent, string sourceAssetPath, int sortOrder, string panelSettingsPath = null)
	{
		UIDocument uIDocument = CreateUIDocument(parent, sortOrder, panelSettingsPath);
		string path = "UI/Views/" + sourceAssetPath;
		uIDocument.visualTreeAsset = _assetLoader.Load<VisualTreeAsset>(path);
		return uIDocument.rootVisualElement;
	}

	public VisualElement Create(string name, string sourceAssetPath, int sortOrder)
	{
		GameObject parent = _rootObjectProvider.CreateRootObject(name);
		return Create(parent, sourceAssetPath, sortOrder);
	}

	public UIDocument CreateEmpty(string name, int sortOrder)
	{
		GameObject parent = _rootObjectProvider.CreateRootObject(name);
		return CreateUIDocument(parent, sortOrder);
	}

	private UIDocument CreateUIDocument(GameObject parent, int sortOrder, string panelSettingsPath = null)
	{
		UIDocument uIDocument = parent.AddComponent<UIDocument>();
		uIDocument.panelSettings = (string.IsNullOrEmpty(panelSettingsPath) ? _assetLoader.Load<PanelSettings>("UI/Views/Core/ScalablePanelSettings") : _assetLoader.Load<PanelSettings>(panelSettingsPath));
		uIDocument.sortingOrder = sortOrder;
		return uIDocument;
	}
}

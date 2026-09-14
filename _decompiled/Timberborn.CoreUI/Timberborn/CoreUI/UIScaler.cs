using Timberborn.AssetSystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class UIScaler : ILoadableSingleton, IUnloadableSingleton
{
	public static readonly float MinScaleFactor = 0.8f;

	public static readonly float MaxScaleFactor = 1.4f;

	private readonly UISettings _uiSettings;

	private readonly IAssetLoader _assetLoader;

	private PanelSettings _panelSettings;

	public UIScaler(UISettings uiSettings, IAssetLoader assetLoader)
	{
		_uiSettings = uiSettings;
		_assetLoader = assetLoader;
	}

	public void Load()
	{
		_panelSettings = _assetLoader.Load<PanelSettings>("UI/Views/Core/ScalablePanelSettings");
		_uiSettings.UIScaleFactorChanged += delegate
		{
			SetScaleFactor();
		};
		SetScaleFactor();
	}

	public void Unload()
	{
		_panelSettings.scale = 1f;
	}

	public float ClampScaleFactor(float value)
	{
		return Mathf.Clamp(value, MinScaleFactor, MaxScaleFactor);
	}

	private void SetScaleFactor()
	{
		_panelSettings.scale = _uiSettings.UIScaleFactor;
	}
}

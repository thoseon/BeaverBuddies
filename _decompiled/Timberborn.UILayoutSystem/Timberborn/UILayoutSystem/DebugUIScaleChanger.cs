using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.UILayoutSystem;

internal class DebugUIScaleChanger : IUpdatableSingleton, IDevModule
{
	private static readonly string IncreaseUIScaleKey = "IncreaseUIScale";

	private static readonly string DecreaseUIScaleKey = "DecreaseUIScale";

	private readonly InputService _inputService;

	private readonly UIScaler _uiScaler;

	private readonly UISettings _uiSettings;

	public DebugUIScaleChanger(InputService inputService, UIScaler uiScaler, UISettings uiSettings)
	{
		_inputService = inputService;
		_uiScaler = uiScaler;
		_uiSettings = uiSettings;
	}

	public void UpdateSingleton()
	{
		if (_inputService.IsKeyHeld(IncreaseUIScaleKey))
		{
			IncreaseUIScale();
		}
		else if (_inputService.IsKeyHeld(DecreaseUIScaleKey))
		{
			DecreaseUIScale();
		}
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.CreateBindable("UI Scale: Increase", IncreaseUIScaleKey, IncreaseUIScale)).AddMethod(DevMethod.CreateBindable("UI Scale: Decrease", DecreaseUIScaleKey, DecreaseUIScale)).Build();
	}

	private void IncreaseUIScale()
	{
		ChangeUIScaleSetting(UISettings.UIScaleStep);
	}

	private void DecreaseUIScale()
	{
		ChangeUIScaleSetting(0f - UISettings.UIScaleStep);
	}

	private void ChangeUIScaleSetting(float change)
	{
		_uiSettings.UIScaleFactor = _uiScaler.ClampScaleFactor(_uiSettings.UIScaleFactor + change);
		Debug.Log($"New UIScale: {_uiSettings.UIScaleFactor:P0}");
	}
}

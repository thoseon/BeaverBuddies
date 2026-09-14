using System;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.MapStateSystem;
using UnityEngine.UIElements;

namespace Timberborn.WaterSourceSystemUI;

internal class WaterSettingFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly DevModeManager _devModeManager;

	private readonly MapEditorMode _mapEditorMode;

	public WaterSettingFactory(VisualElementLoader visualElementLoader, DevModeManager devModeManager, MapEditorMode mapEditorMode)
	{
		_visualElementLoader = visualElementLoader;
		_devModeManager = devModeManager;
		_mapEditorMode = mapEditorMode;
	}

	public WaterSetting Create(string label, Action<float> setter, Func<float> getter, bool devModeOnly)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/EntityPanel/WaterSetting");
		visualElement.Q<Label>("Text").text = label;
		FloatField inputField = visualElement.Q<FloatField>("Value");
		inputField.RegisterValueChangedCallback(delegate(ChangeEvent<float> value)
		{
			setter(value.newValue);
			inputField.SetValueWithoutNotify(getter());
			inputField.Blur();
		});
		inputField.isDelayed = true;
		return new WaterSetting(_devModeManager, _mapEditorMode, visualElement, inputField, getter, devModeOnly);
	}
}

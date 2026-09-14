using System;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.MapStateSystem;
using UnityEngine.UIElements;

namespace Timberborn.ActivatorSystemUI;

internal class TimedActivatorSettingFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly DevModeManager _devModeManager;

	private readonly MapEditorMode _mapEditorMode;

	public TimedActivatorSettingFactory(VisualElementLoader visualElementLoader, DevModeManager devModeManager, MapEditorMode mapEditorMode)
	{
		_visualElementLoader = visualElementLoader;
		_devModeManager = devModeManager;
		_mapEditorMode = mapEditorMode;
	}

	public TimedActivatorSetting Create(string label, Action<float> setter, Func<float> getter, float minValue)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/EntityPanel/TimedActivatorSetting");
		visualElement.Q<Label>("Text").text = label;
		FloatField inputField = visualElement.Q<FloatField>("Value");
		TextFields.InitializeFloatField(inputField, minValue, minValue, float.MaxValue, delegate(float value)
		{
			OnInputValueChanged(setter, value, inputField);
		});
		inputField.isDelayed = true;
		return new TimedActivatorSetting(_devModeManager, _mapEditorMode, visualElement, inputField, getter);
	}

	private static void OnInputValueChanged(Action<float> setter, float value, FloatField inputField)
	{
		setter(value);
		inputField.Blur();
	}
}

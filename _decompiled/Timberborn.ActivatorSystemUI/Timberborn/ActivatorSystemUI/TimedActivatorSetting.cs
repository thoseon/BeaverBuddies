using System;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.MapStateSystem;
using UnityEngine.UIElements;

namespace Timberborn.ActivatorSystemUI;

internal class TimedActivatorSetting
{
	private readonly DevModeManager _devModeManager;

	private readonly MapEditorMode _mapEditorMode;

	private readonly FloatField _floatField;

	private readonly Func<float> _getter;

	public VisualElement Root { get; }

	private bool Visible
	{
		get
		{
			if (!_devModeManager.Enabled)
			{
				return _mapEditorMode.IsMapEditor;
			}
			return true;
		}
	}

	public TimedActivatorSetting(DevModeManager devModeManager, MapEditorMode mapEditorMode, VisualElement root, FloatField floatField, Func<float> getter)
	{
		_devModeManager = devModeManager;
		_mapEditorMode = mapEditorMode;
		Root = root;
		_floatField = floatField;
		_getter = getter;
	}

	public void UpdateState()
	{
		Root.ToggleDisplayStyle(Visible);
		if (!_floatField.IsFocused())
		{
			_floatField.SetValueWithoutNotify(_getter());
		}
	}
}

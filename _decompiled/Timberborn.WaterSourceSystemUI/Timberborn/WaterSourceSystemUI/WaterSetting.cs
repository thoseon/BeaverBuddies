using System;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.MapStateSystem;
using UnityEngine.UIElements;

namespace Timberborn.WaterSourceSystemUI;

internal class WaterSetting
{
	private readonly DevModeManager _devModeManager;

	private readonly MapEditorMode _mapEditorMode;

	private readonly FloatField _inputField;

	private readonly Func<float> _getter;

	private readonly bool _devModeOnly;

	public VisualElement Root { get; }

	public bool Visible
	{
		get
		{
			if (!_devModeManager.Enabled)
			{
				if (!_devModeOnly)
				{
					return _mapEditorMode.IsMapEditor;
				}
				return false;
			}
			return true;
		}
	}

	public WaterSetting(DevModeManager devModeManager, MapEditorMode mapEditorMode, VisualElement root, FloatField inputField, Func<float> getter, bool devModeOnly)
	{
		_devModeManager = devModeManager;
		_mapEditorMode = mapEditorMode;
		Root = root;
		_inputField = inputField;
		_getter = getter;
		_devModeOnly = devModeOnly;
	}

	public void UpdateState()
	{
		_inputField.parent.ToggleDisplayStyle(Visible);
		if (!_inputField.IsFocused())
		{
			_inputField.SetValueWithoutNotify(_getter());
		}
	}
}

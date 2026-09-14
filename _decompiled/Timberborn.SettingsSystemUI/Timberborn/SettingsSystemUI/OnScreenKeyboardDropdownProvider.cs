using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.DropdownSystem;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;

namespace Timberborn.SettingsSystemUI;

internal class OnScreenKeyboardDropdownProvider : IDropdownProvider, ILoadableSingleton
{
	private readonly InputSettings _inputSettings;

	private readonly ILoc _loc;

	private ImmutableArray<string> _valuesFormatted;

	public IReadOnlyList<string> Items => _valuesFormatted;

	public OnScreenKeyboardDropdownProvider(InputSettings inputSettings, ILoc loc)
	{
		_inputSettings = inputSettings;
		_loc = loc;
	}

	public string GetValue()
	{
		return GetFormattedValue(_inputSettings.OnScreenKeyboard);
	}

	public void SetValue(string value)
	{
		_inputSettings.OnScreenKeyboard = InputSettings.OnScreenKeyboardValues[_valuesFormatted.IndexOf(value)];
	}

	public void Load()
	{
		_valuesFormatted = InputSettings.OnScreenKeyboardValues.Select(GetFormattedValue).ToImmutableArray();
	}

	private string GetFormattedValue(string value)
	{
		return _loc.T("Settings.Input.OnScreenKeyboard." + value);
	}
}

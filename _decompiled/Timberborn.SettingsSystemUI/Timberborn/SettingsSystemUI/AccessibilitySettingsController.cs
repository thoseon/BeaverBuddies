using Timberborn.AccessibilitySettingsSystem;
using UnityEngine.UIElements;

namespace Timberborn.SettingsSystemUI;

internal class AccessibilitySettingsController
{
	private readonly AccessibilitySettings _accessibilitySettings;

	private Toggle _disableStarfieldRotation;

	public AccessibilitySettingsController(AccessibilitySettings accessibilitySettings)
	{
		_accessibilitySettings = accessibilitySettings;
	}

	public void Initialize(VisualElement root)
	{
		_disableStarfieldRotation = root.Q<Toggle>("DisableStarfieldRotation");
		_disableStarfieldRotation.RegisterValueChangedCallback(delegate(ChangeEvent<bool> v)
		{
			_accessibilitySettings.StarfieldRotationDisabled = v.newValue;
		});
	}

	public void Update()
	{
		_disableStarfieldRotation.SetValueWithoutNotify(_accessibilitySettings.StarfieldRotationDisabled);
	}
}

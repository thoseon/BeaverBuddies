using Timberborn.SettingsSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.AccessibilitySettingsSystem;

public class AccessibilitySettings : IPostLoadableSingleton
{
	private static readonly int StarfieldRotationDisabledProperty = Shader.PropertyToID("_StarfieldRotationDisabled");

	private static readonly string StarfieldRotationDisabledKey = "StarfieldRotationDisabled";

	private readonly ISettings _settings;

	public bool StarfieldRotationDisabled
	{
		get
		{
			return _settings.GetBool(StarfieldRotationDisabledKey);
		}
		set
		{
			_settings.SetBool(StarfieldRotationDisabledKey, value);
			UpdateShaderProperties();
		}
	}

	public AccessibilitySettings(ISettings settings)
	{
		_settings = settings;
	}

	public void PostLoad()
	{
		UpdateShaderProperties();
	}

	private void UpdateShaderProperties()
	{
		Shader.SetGlobalInt(StarfieldRotationDisabledProperty, StarfieldRotationDisabled ? 1 : 0);
	}
}

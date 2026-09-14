using Timberborn.CameraSettingsSystem;
using UnityEngine.UIElements;

namespace Timberborn.SettingsSystemUI;

internal class CameraSettingsController
{
	private readonly CameraSettings _cameraSettings;

	private Toggle _unlockZoomToggle;

	public CameraSettingsController(CameraSettings cameraSettings)
	{
		_cameraSettings = cameraSettings;
	}

	public void Initialize(VisualElement root)
	{
		_unlockZoomToggle = root.Q<Toggle>("UnlockZoom");
		_unlockZoomToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> v)
		{
			_cameraSettings.UnlockZoom = v.newValue;
		});
	}

	public void Update()
	{
		_unlockZoomToggle.SetValueWithoutNotify(_cameraSettings.UnlockZoom);
	}
}

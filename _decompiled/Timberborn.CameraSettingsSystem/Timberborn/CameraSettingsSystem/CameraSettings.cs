using Timberborn.SettingsSystem;

namespace Timberborn.CameraSettingsSystem;

public class CameraSettings
{
	private static readonly string UnlockZoomKey = "UnlockZoom";

	private readonly ISettings _settings;

	public bool UnlockZoom
	{
		get
		{
			return _settings.GetBool(UnlockZoomKey);
		}
		set
		{
			_settings.SetBool(UnlockZoomKey, value);
		}
	}

	public CameraSettings(ISettings settings)
	{
		_settings = settings;
	}
}

using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.ApplicationSettingsSystem;

internal class RunInBackgroundController : ILoadableSingleton
{
	private readonly UISettings _uiSettings;

	public RunInBackgroundController(UISettings uiSettings)
	{
		_uiSettings = uiSettings;
	}

	public void Load()
	{
		UpdateSetting();
		_uiSettings.RunInBackgroundChanged += delegate
		{
			UpdateSetting();
		};
	}

	private void UpdateSetting()
	{
		Application.runInBackground = _uiSettings.RunInBackground;
	}
}

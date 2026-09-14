using Timberborn.TutorialSettingsSystem;
using UnityEngine.UIElements;

namespace Timberborn.SettingsSystemUI;

internal class TutorialSettingsController
{
	private readonly TutorialSettings _tutorialSettings;

	private Toggle _disableTutorialToggle;

	public TutorialSettingsController(TutorialSettings tutorialSettings)
	{
		_tutorialSettings = tutorialSettings;
	}

	public void Initialize(VisualElement root)
	{
		_disableTutorialToggle = root.Q<Toggle>("DisableTutorial");
		_disableTutorialToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> v)
		{
			_tutorialSettings.DisableTutorial = v.newValue;
		});
	}

	public void Update()
	{
		_disableTutorialToggle.SetValueWithoutNotify(_tutorialSettings.DisableTutorial);
	}
}

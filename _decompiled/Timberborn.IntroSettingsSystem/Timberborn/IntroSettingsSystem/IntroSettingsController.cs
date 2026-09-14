using UnityEngine.UIElements;

namespace Timberborn.IntroSettingsSystem;

public class IntroSettingsController
{
	private readonly IntroSettings _introSettings;

	private Toggle _disableIntroToggle;

	public IntroSettingsController(IntroSettings introSettings)
	{
		_introSettings = introSettings;
	}

	public void Initialize(VisualElement root)
	{
		_disableIntroToggle = root.Q<Toggle>("DisableIntro");
		_disableIntroToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> v)
		{
			_introSettings.DisableIntro = v.newValue;
		});
	}

	public void Update()
	{
		_disableIntroToggle.SetValueWithoutNotify(_introSettings.DisableIntro);
	}
}

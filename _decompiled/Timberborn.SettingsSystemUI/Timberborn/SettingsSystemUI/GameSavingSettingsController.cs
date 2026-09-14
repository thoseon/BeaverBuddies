using UnityEngine.UIElements;

namespace Timberborn.SettingsSystemUI;

internal class GameSavingSettingsController
{
	private readonly GameSavingSettings _gameSavingSetting;

	private Toggle _autoSavingOnToggle;

	public GameSavingSettingsController(GameSavingSettings gameSavingSetting)
	{
		_gameSavingSetting = gameSavingSetting;
	}

	public void Initialize(VisualElement root)
	{
		_autoSavingOnToggle = root.Q<Toggle>("AutoSavingOn");
		_autoSavingOnToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> v)
		{
			_gameSavingSetting.AutoSavingOn = v.newValue;
		});
	}

	public void Update()
	{
		_autoSavingOnToggle.SetValueWithoutNotify(_gameSavingSetting.AutoSavingOn);
	}
}

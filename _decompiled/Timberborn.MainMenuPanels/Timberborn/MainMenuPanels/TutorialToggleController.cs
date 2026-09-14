using Timberborn.CoreUI;
using Timberborn.FactionSystem;
using Timberborn.TutorialSettingsSystem;
using UnityEngine.UIElements;

namespace Timberborn.MainMenuPanels;

public class TutorialToggleController
{
	private readonly TutorialSettings _tutorialSettings;

	private Toggle _tutorialToggle;

	private VisualElement _tutorialToggleWrapper;

	private Toggle _tutorialToggleCustom;

	private VisualElement _tutorialToggleCustomWrapper;

	private bool _showToggles;

	private bool _showMainToggle;

	public TutorialToggleController(TutorialSettings tutorialSettings)
	{
		_tutorialSettings = tutorialSettings;
	}

	internal void Initialize(VisualElement root)
	{
		_tutorialToggleWrapper = root.Q<VisualElement>("TutorialToggleWrapper");
		_tutorialToggle = root.Q<Toggle>("TutorialToggle");
		_tutorialToggleCustomWrapper = root.Q<VisualElement>("TutorialToggleCustomWrapper");
		_tutorialToggleCustom = root.Q<Toggle>("TutorialToggleCustom");
		UpdateToggles();
		_tutorialToggle.RegisterValueChangedCallback(OnToggleChanged);
		_tutorialToggleCustom.RegisterValueChangedCallback(OnToggleChanged);
	}

	internal void SetFaction(FactionSpec factionSpec)
	{
		_showToggles = factionSpec.HasSpec<StartingFactionSpec>();
		UpdateTogglesVisibility();
		UpdateToggles();
	}

	internal void ShowMainToggle()
	{
		_showMainToggle = true;
		UpdateTogglesVisibility();
	}

	internal void HideMainToggle()
	{
		_showMainToggle = false;
		UpdateTogglesVisibility();
	}

	private void OnToggleChanged(ChangeEvent<bool> evt)
	{
		_tutorialSettings.DisableTutorial = !evt.newValue;
		UpdateToggles();
	}

	private void UpdateTogglesVisibility()
	{
		_tutorialToggleWrapper.ToggleDisplayStyle(_showToggles && _showMainToggle);
		_tutorialToggleCustomWrapper.ToggleDisplayStyle(_showToggles && !_showMainToggle);
	}

	private void UpdateToggles()
	{
		_tutorialToggle.SetValueWithoutNotify(!_tutorialSettings.DisableTutorial);
		_tutorialToggleCustom.SetValueWithoutNotify(!_tutorialSettings.DisableTutorial);
	}
}

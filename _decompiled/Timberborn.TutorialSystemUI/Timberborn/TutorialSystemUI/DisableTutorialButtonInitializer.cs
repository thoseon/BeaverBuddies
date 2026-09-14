using Timberborn.CoreUI;
using Timberborn.TooltipSystem;
using Timberborn.TutorialSettingsSystem;
using UnityEngine.UIElements;

namespace Timberborn.TutorialSystemUI;

public class DisableTutorialButtonInitializer
{
	private static readonly string EnableHoverClass = "hover-enabled";

	private static readonly string DisableLocKey = "Tutorial.Disable";

	private static readonly string DisablePromptLocKey = "Tutorial.DisablePrompt";

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly TutorialSettings _tutorialSettings;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	public DisableTutorialButtonInitializer(DialogBoxShower dialogBoxShower, TutorialSettings tutorialSettings, ITooltipRegistrar tooltipRegistrar)
	{
		_dialogBoxShower = dialogBoxShower;
		_tutorialSettings = tutorialSettings;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public void Initialize(VisualElement root)
	{
		VisualElement header = root.Q<VisualElement>("TutorialHeader");
		Button button = root.Q<Button>("Disable");
		button.RegisterCallback<ClickEvent>(OnSkipTutorialClicked);
		header.AddToClassList(EnableHoverClass);
		button.RegisterCallback<MouseEnterEvent>(delegate
		{
			header.RemoveFromClassList(EnableHoverClass);
		});
		button.RegisterCallback<MouseLeaveEvent>(delegate
		{
			header.AddToClassList(EnableHoverClass);
		});
		_tooltipRegistrar.RegisterLocalizable(button, DisableLocKey);
	}

	private void OnSkipTutorialClicked(ClickEvent evt)
	{
		_dialogBoxShower.Create().SetLocalizedMessage(DisablePromptLocKey).SetConfirmButton(DisableTutorial)
			.SetDefaultCancelButton()
			.Show();
	}

	private void DisableTutorial()
	{
		_tutorialSettings.DisableTutorial = true;
	}
}

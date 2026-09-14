using Timberborn.TutorialSystem;
using UnityEngine.UIElements;

namespace Timberborn.TutorialSystemUI;

internal class AchievedStepsController
{
	private static readonly string AllStepsAchievedClass = "all-steps-achieved";

	private readonly ITutorialService _tutorialService;

	private readonly VisualElement _root;

	private readonly Button _button;

	private TutorialStage _activeTutorialStage;

	public AchievedStepsController(ITutorialService tutorialService, VisualElement root, Button button)
	{
		_tutorialService = tutorialService;
		_root = root;
		_button = button;
	}

	public void Initialize(string tutorialId)
	{
		_button.RegisterCallback<ClickEvent>(delegate
		{
			_tutorialService.StartNextStage(tutorialId);
		});
	}

	public void ChangeActiveTutorialStage(TutorialStage tutorialStage)
	{
		_activeTutorialStage = tutorialStage;
		UpdateVisibility();
	}

	public void UpdateVisibility()
	{
		bool enable = _activeTutorialStage?.AllStepsAchieved ?? false;
		_root.EnableInClassList(AllStepsAchievedClass, enable);
	}
}

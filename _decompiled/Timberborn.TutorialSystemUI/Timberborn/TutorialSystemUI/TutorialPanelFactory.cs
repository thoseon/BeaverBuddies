using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.SingletonSystem;
using Timberborn.TutorialSystem;
using UnityEngine.UIElements;

namespace Timberborn.TutorialSystemUI;

internal class TutorialPanelFactory
{
	private readonly DisableTutorialButtonInitializer _disableTutorialButtonInitializer;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly EventBus _eventBus;

	private readonly DevModeManager _devModeManager;

	private readonly ITutorialService _tutorialService;

	private readonly TutorialPanelBlinker _tutorialPanelBlinker;

	private readonly TutorialStepViewFactory _tutorialStepViewFactory;

	public TutorialPanelFactory(DisableTutorialButtonInitializer disableTutorialButtonInitializer, VisualElementLoader visualElementLoader, EventBus eventBus, DevModeManager devModeManager, ITutorialService tutorialService, TutorialPanelBlinker tutorialPanelBlinker, TutorialStepViewFactory tutorialStepViewFactory)
	{
		_disableTutorialButtonInitializer = disableTutorialButtonInitializer;
		_visualElementLoader = visualElementLoader;
		_eventBus = eventBus;
		_devModeManager = devModeManager;
		_tutorialService = tutorialService;
		_tutorialPanelBlinker = tutorialPanelBlinker;
		_tutorialStepViewFactory = tutorialStepViewFactory;
	}

	public TutorialPanel Create(TutorialConfiguration tutorialConfiguration)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/Tutorial/TutorialPanel");
		_disableTutorialButtonInitializer.Initialize(visualElement);
		visualElement.Q<Label>("TutorialTitle").text = tutorialConfiguration.DisplayName;
		visualElement.Q<Button>("HeaderButton").RegisterCallback<ClickEvent>(delegate
		{
			OnHeaderClicked(tutorialConfiguration.TutorialId);
		});
		AchievedStepsController achievedStepsController = new AchievedStepsController(_tutorialService, visualElement, visualElement.Q<Button>("Continue"));
		TutorialPanel tutorialPanel = new TutorialPanel(_tutorialService, achievedStepsController, _eventBus, _devModeManager, _tutorialPanelBlinker, visualElement, tutorialConfiguration, _tutorialStepViewFactory);
		tutorialPanel.Initialize();
		return tutorialPanel;
	}

	private void OnHeaderClicked(string tutorialId)
	{
		_eventBus.Post(new TutorialHeaderClickedEvent(tutorialId));
	}
}

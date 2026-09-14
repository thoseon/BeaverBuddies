using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.SingletonSystem;
using Timberborn.TutorialSystem;
using UnityEngine.UIElements;

namespace Timberborn.TutorialSystemUI;

internal class TutorialPanel
{
	private static readonly string HiddenClass = "hidden";

	private readonly ITutorialService _tutorialService;

	private readonly AchievedStepsController _achievedStepsController;

	private readonly EventBus _eventBus;

	private readonly DevModeManager _devModeManager;

	private readonly TutorialPanelBlinker _tutorialPanelBlinker;

	private readonly TutorialConfiguration _tutorialConfiguration;

	private readonly TutorialStepViewFactory _tutorialStepViewFactory;

	private string _tutorialId;

	private Label _stageState;

	private Label _intro;

	private VisualElement _tutorialSteps;

	private readonly List<TutorialStepView> _tutorialStepViews = new List<TutorialStepView>();

	private Button _devCompleteButton;

	private Button _devCompleteAllButton;

	private bool _enabled;

	public bool IsVisible { get; private set; }

	public VisualElement Root { get; }

	public int SortOrder => _tutorialConfiguration.SortOrder;

	public TutorialPanel(ITutorialService tutorialService, AchievedStepsController achievedStepsController, EventBus eventBus, DevModeManager devModeManager, TutorialPanelBlinker tutorialPanelBlinker, VisualElement root, TutorialConfiguration tutorialConfiguration, TutorialStepViewFactory tutorialStepViewFactory)
	{
		_tutorialService = tutorialService;
		_achievedStepsController = achievedStepsController;
		_eventBus = eventBus;
		_devModeManager = devModeManager;
		_tutorialPanelBlinker = tutorialPanelBlinker;
		Root = root;
		_tutorialConfiguration = tutorialConfiguration;
		_tutorialStepViewFactory = tutorialStepViewFactory;
	}

	public void Initialize()
	{
		_stageState = Root.Q<Label>("TutorialStageState");
		_intro = Root.Q<Label>("Intro");
		_tutorialId = _tutorialConfiguration.TutorialId;
		_achievedStepsController.Initialize(_tutorialId);
		_tutorialSteps = Root.Q<VisualElement>("TutorialSteps");
		_devCompleteButton = Root.Q<Button>("DevComplete");
		_devCompleteButton.RegisterCallback<ClickEvent>(delegate
		{
			ForceComplete();
		});
		_devCompleteAllButton = Root.Q<Button>("DevCompleteAll");
		_devCompleteAllButton.RegisterCallback<ClickEvent>(delegate
		{
			ForceCompleteAll();
		});
		_eventBus.Register(this);
		_enabled = true;
		_tutorialPanelBlinker.StartBlinking(Root, _tutorialConfiguration.KeepBlinking);
		Root.ToggleDisplayStyle(visible: true);
		Hide();
	}

	public void Disable()
	{
		_tutorialPanelBlinker.StopBlinking(Root);
		_enabled = false;
		_eventBus.Unregister((object)this);
	}

	[OnEvent]
	public void OnTutorialStageStarted(TutorialStageStartedEvent tutorialStageStartedEvent)
	{
		if (tutorialStageStartedEvent.TutorialId == _tutorialId)
		{
			_intro.text = tutorialStageStartedEvent.TutorialStage.Intro;
			TransitionIntoNextStage(tutorialStageStartedEvent.TutorialStage);
			_achievedStepsController.ChangeActiveTutorialStage(tutorialStageStartedEvent.TutorialStage);
		}
	}

	[OnEvent]
	public void OnTutorialHeaderClicked(TutorialHeaderClickedEvent tutorialHeaderClickedEvent)
	{
		if (_tutorialId == tutorialHeaderClickedEvent.TutorialId && !IsVisible)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	public void Update()
	{
		_achievedStepsController.UpdateVisibility();
		int num = 0;
		foreach (TutorialStepView tutorialStepView in _tutorialStepViews)
		{
			tutorialStepView.Update();
			num += (tutorialStepView.IsAchieved ? 1 : 0);
		}
		_stageState.text = $"{num}/{_tutorialStepViews.Count}";
		UpdateDevButtons();
	}

	public void UnhighlightAssociatedTools()
	{
		foreach (TutorialStepView tutorialStepView in _tutorialStepViews)
		{
			tutorialStepView.UnhighlightAssociatedTools();
		}
	}

	private void TransitionIntoNextStage(TutorialStage tutorialStage)
	{
		DeactivateActiveStage();
		ActivateStage(tutorialStage);
	}

	private void DeactivateActiveStage()
	{
		UnhighlightAssociatedTools();
		_tutorialSteps.Clear();
		_tutorialStepViews.Clear();
	}

	private void ActivateStage(TutorialStage tutorialStage)
	{
		AddTutorialStepViews(tutorialStage.TutorialSteps);
		_stageState.ToggleDisplayStyle(_tutorialStepViews.Count > 0);
	}

	private void AddTutorialStepViews(IEnumerable<TutorialStep> tutorialSteps)
	{
		foreach (TutorialStep tutorialStep in tutorialSteps)
		{
			AddTutorialStepView(tutorialStep);
		}
	}

	private void AddTutorialStepView(TutorialStep tutorialStep)
	{
		TutorialStepView tutorialStepView = _tutorialStepViewFactory.Create(tutorialStep, this);
		_tutorialStepViews.Add(tutorialStepView);
		_tutorialSteps.Add(tutorialStepView.Root);
	}

	private void Show()
	{
		_tutorialPanelBlinker.StopBlinking(Root);
		IsVisible = true;
		Root.RemoveFromClassList(HiddenClass);
		Update();
	}

	private void Hide()
	{
		UnhighlightAssociatedTools();
		Root.AddToClassList(HiddenClass);
		IsVisible = false;
		UpdateDevButtons();
	}

	private void UpdateDevButtons()
	{
		bool visible = _devModeManager.Enabled && IsVisible;
		_devCompleteButton.ToggleDisplayStyle(visible);
		_devCompleteAllButton.ToggleDisplayStyle(visible);
	}

	private void ForceComplete()
	{
		UnhighlightAssociatedTools();
		_tutorialService.StartNextStage(_tutorialId);
	}

	private void ForceCompleteAll()
	{
		UnhighlightAssociatedTools();
		while (_enabled)
		{
			_tutorialService.StartNextStage(_tutorialId);
		}
	}
}

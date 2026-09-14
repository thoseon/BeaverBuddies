using System.Collections.Generic;
using System.Linq;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.TutorialSettingsSystem;
using Timberborn.TutorialSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.TutorialSystemUI;

internal class TutorialPanels : ILoadableSingleton, IUpdatableSingleton
{
	private readonly UILayout _uiLayout;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly EventBus _eventBus;

	private readonly TutorialSettings _tutorialSettings;

	private readonly TutorialPanelFactory _tutorialPanelFactory;

	private VisualElement _root;

	private readonly Dictionary<string, TutorialPanel> _tutorialPanels = new Dictionary<string, TutorialPanel>();

	private bool _enabled;

	private bool TutorialIsOn
	{
		get
		{
			if (_enabled && !_tutorialSettings.DisableTutorial)
			{
				return _tutorialPanels.Count > 0;
			}
			return false;
		}
	}

	public TutorialPanels(UILayout uiLayout, VisualElementLoader visualElementLoader, EventBus eventBus, TutorialSettings tutorialSettings, TutorialPanelFactory tutorialPanelFactory)
	{
		_uiLayout = uiLayout;
		_visualElementLoader = visualElementLoader;
		_eventBus = eventBus;
		_tutorialSettings = tutorialSettings;
		_tutorialPanelFactory = tutorialPanelFactory;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/Tutorial/TutorialPanels");
		_uiLayout.AddBottomRight(_root, 4);
		_eventBus.Register(this);
		_tutorialSettings.DisableTutorialChanged += delegate
		{
			ShowIfConditionsMet();
		};
		Hide();
	}

	public void UpdateSingleton()
	{
		if (!TutorialIsOn)
		{
			return;
		}
		foreach (TutorialPanel value in _tutorialPanels.Values)
		{
			value.Update();
		}
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_enabled = true;
		ShowIfConditionsMet();
	}

	[OnEvent]
	public void OnTutorialCreated(TutorialCreatedEvent tutorialCreatedEvent)
	{
		TutorialConfiguration configuration = tutorialCreatedEvent.Configuration;
		TutorialPanel tutorialPanel = _tutorialPanelFactory.Create(configuration);
		_tutorialPanels.Add(configuration.TutorialId, tutorialPanel);
		_root.Add(tutorialPanel.Root);
		ShowIfConditionsMet();
	}

	[OnEvent]
	public void OnTutorialFinished(TutorialFinishedEvent tutorialFinishedEvent)
	{
		string tutorialId = tutorialFinishedEvent.TutorialId;
		TutorialPanel tutorialPanel = _tutorialPanels[tutorialId];
		tutorialPanel.Disable();
		_root.Remove(tutorialPanel.Root);
		_tutorialPanels.Remove(tutorialId);
		if (_tutorialPanels.Count == 0)
		{
			Hide();
		}
	}

	private void ShowIfConditionsMet()
	{
		if (TutorialIsOn)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	private void Show()
	{
		_root.ToggleDisplayStyle(visible: true);
		SortTutorialPanels();
	}

	private void Hide()
	{
		_root.ToggleDisplayStyle(visible: false);
		foreach (TutorialPanel value in _tutorialPanels.Values)
		{
			if (value.IsVisible)
			{
				value.UnhighlightAssociatedTools();
			}
		}
	}

	private void SortTutorialPanels()
	{
		foreach (TutorialPanel item in _tutorialPanels.Values.OrderBy((TutorialPanel panel) => panel.SortOrder))
		{
			item.Root.BringToFront();
		}
	}
}

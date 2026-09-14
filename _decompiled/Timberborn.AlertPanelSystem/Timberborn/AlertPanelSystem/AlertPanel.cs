using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.AlertPanelSystem;

internal class AlertPanel : ILoadableSingleton, IUpdatableSingleton
{
	private readonly UILayout _uiLayout;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly EventBus _eventBus;

	private readonly ImmutableArray<AlertPanelModule> _alertPanelModules;

	private readonly List<IAlertFragment> _alertFragments = new List<IAlertFragment>();

	private VisualElement _root;

	private bool _enabled;

	public AlertPanel(UILayout uiLayout, VisualElementLoader visualElementLoader, EventBus eventBus, IEnumerable<AlertPanelModule> alertPanelModules)
	{
		_uiLayout = uiLayout;
		_visualElementLoader = visualElementLoader;
		_eventBus = eventBus;
		_alertPanelModules = alertPanelModules.ToImmutableArray();
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/AlertPanel/AlertPanel");
		AddAlertFragments(_root);
		_eventBus.Register(this);
	}

	public void UpdateSingleton()
	{
		if (_enabled)
		{
			UpdateAlertFragments();
		}
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_uiLayout.AddBottomLeft(_root, 1);
		_enabled = true;
		UpdateAlertFragments();
	}

	private void AddAlertFragments(VisualElement root)
	{
		Dictionary<int, IAlertFragment> dictionary = new Dictionary<int, IAlertFragment>();
		foreach (AlertPanelModule alertPanelModule in _alertPanelModules)
		{
			foreach (KeyValuePair<int, IAlertFragment> alertFragment2 in alertPanelModule.AlertFragments)
			{
				dictionary.Add(alertFragment2.Key, alertFragment2.Value);
			}
		}
		foreach (int item in dictionary.Keys.OrderBy((int key) => key))
		{
			IAlertFragment alertFragment = dictionary[item];
			_alertFragments.Add(alertFragment);
			alertFragment.InitializeAlertFragment(root);
		}
	}

	private void UpdateAlertFragments()
	{
		foreach (IAlertFragment alertFragment in _alertFragments)
		{
			alertFragment.UpdateAlertFragment();
		}
	}
}

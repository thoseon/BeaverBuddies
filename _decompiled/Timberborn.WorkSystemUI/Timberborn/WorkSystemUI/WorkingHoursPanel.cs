using System;
using Timberborn.CoreUI;
using Timberborn.InputSystemUI;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.TooltipSystem;
using Timberborn.UIFormatters;
using Timberborn.UILayoutSystem;
using Timberborn.WorkSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.WorkSystemUI;

public class WorkingHoursPanel : ILoadableSingleton
{
	private static readonly string WorkingHoursTooltipLocKey = "Work.WorkingHoursTooltip";

	private static readonly string IncreaseHoursKey = "IncreaseWorkingHours";

	private static readonly string DecreaseHoursKey = "DecreaseWorkingHours";

	private static readonly string HighlightClass = "highlight";

	private readonly UILayout _uiLayout;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly WorkingHoursManager _workingHoursManager;

	private readonly ILoc _loc;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly BindableButtonFactory _bindableButtonFactory;

	private readonly EventBus _eventBus;

	private BindableButton _increaseHoursButton;

	private BindableButton _decreaseHoursButton;

	private Label _title;

	private int _hours;

	private VisualElement _root;

	private VisualElement _workingHoursPanel;

	private readonly Phrase _titlePhrase = Phrase.New().FormatHours<int>("F0");

	public WorkingHoursPanel(UILayout uiLayout, VisualElementLoader visualElementLoader, WorkingHoursManager workingHoursManager, ILoc loc, ITooltipRegistrar tooltipRegistrar, BindableButtonFactory bindableButtonFactory, EventBus eventBus)
	{
		_uiLayout = uiLayout;
		_visualElementLoader = visualElementLoader;
		_workingHoursManager = workingHoursManager;
		_loc = loc;
		_tooltipRegistrar = tooltipRegistrar;
		_bindableButtonFactory = bindableButtonFactory;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/WorkingHoursPanel");
		_workingHoursPanel = _root.Q<VisualElement>("WorkingHours");
		_tooltipRegistrar.RegisterLocalizable(_root, WorkingHoursTooltipLocKey);
		_increaseHoursButton = _bindableButtonFactory.CreateAndBind(_root.Q<Button>("Plus"), IncreaseHoursKey, IncreaseHours);
		_decreaseHoursButton = _bindableButtonFactory.CreateAndBind(_root.Q<Button>("Minus"), DecreaseHoursKey, DecreaseHours);
		_title = _root.Q<Label>("Text");
		_hours = Mathf.CeilToInt(_workingHoursManager.WorkedPartOfDay * 24f);
		UpdateTitle();
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_uiLayout.AddTopRight(_root, 3);
	}

	public void TogglePanelHighlight(bool state)
	{
		_workingHoursPanel.EnableInClassList(HighlightClass, state);
	}

	private void IncreaseHours()
	{
		_hours = Math.Min(24, _hours + 1);
		_decreaseHoursButton.Enable();
		if (_hours == 24)
		{
			_increaseHoursButton.Disable();
		}
		OnHoursChanged();
	}

	private void DecreaseHours()
	{
		_hours = Math.Max(0, _hours - 1);
		_increaseHoursButton.Enable();
		if (_hours == 0)
		{
			_decreaseHoursButton.Disable();
		}
		OnHoursChanged();
	}

	private void OnHoursChanged()
	{
		_workingHoursManager.WorkedPartOfDay = (float)_hours / 24f;
		UpdateTitle();
	}

	private void UpdateTitle()
	{
		_title.text = _loc.T(_titlePhrase, _hours);
	}
}

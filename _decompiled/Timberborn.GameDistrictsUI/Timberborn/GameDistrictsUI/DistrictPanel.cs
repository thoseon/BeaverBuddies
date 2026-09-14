using System.Collections.Generic;
using Timberborn.BatchControl;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.EntityNaming;
using Timberborn.GameDistricts;
using Timberborn.InputSystemUI;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.TooltipSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsUI;

internal class DistrictPanel : ILoadableSingleton, IHideableByBatchControl
{
	private static readonly string HiddenClass = "extension-clamp--hidden";

	private static readonly string PanelDistrictClass = "panel--district";

	private static readonly string GlobalViewLocKey = "Districts.GlobalView";

	private static readonly string PreviousDistrictKey = "PreviousDistrict";

	private static readonly string ShowDistrictListKey = "GameUI.ShowDistrictList";

	private static readonly string NextDistrictKey = "NextDistrict";

	private readonly UILayout _uiLayout;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly DistrictContextService _districtContextService;

	private readonly DistrictListPanel _districtListPanel;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly ILoc _loc;

	private readonly EventBus _eventBus;

	private readonly BindableButtonFactory _bindableButtonFactory;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private Button _districtNameButton;

	private Button _extensionToggler;

	private bool _districtSelectionToggled;

	private VisualElement _districtHeader;

	private VisualElement _root;

	private IReadOnlyList<DistrictCenter> DistrictCenters => _districtCenterRegistry.FinishedDistrictCenters;

	private int IndexOfCurrentlySelectedDistrict => DistrictCenters.IndexOf(_districtContextService.SelectedDistrict);

	public DistrictPanel(UILayout uiLayout, VisualElementLoader visualElementLoader, DistrictContextService districtContextService, DistrictListPanel districtListPanel, EntitySelectionService entitySelectionService, DistrictCenterRegistry districtCenterRegistry, ILoc loc, EventBus eventBus, BindableButtonFactory bindableButtonFactory, ITooltipRegistrar tooltipRegistrar)
	{
		_uiLayout = uiLayout;
		_visualElementLoader = visualElementLoader;
		_districtContextService = districtContextService;
		_districtListPanel = districtListPanel;
		_entitySelectionService = entitySelectionService;
		_districtCenterRegistry = districtCenterRegistry;
		_loc = loc;
		_eventBus = eventBus;
		_bindableButtonFactory = bindableButtonFactory;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/Districts/DistrictPanel");
		_districtHeader = _root.Q<VisualElement>("DistrictHeader");
		_districtNameButton = _root.Q<Button>("DistrictName");
		_districtNameButton.RegisterCallback<ClickEvent>(ToggleDistrictSelection);
		ResetDistrictNameButtonLabel();
		_extensionToggler = _root.Q<Button>("ExtensionToggler");
		_extensionToggler.RegisterCallback<ClickEvent>(ToggleDistrictSelection);
		_extensionToggler.AddToClassList(HiddenClass);
		Button button = _root.Q<Button>("PreviousDistrict");
		Button button2 = _root.Q<Button>("NextDistrict");
		_bindableButtonFactory.CreateAndBind(button, PreviousDistrictKey, SelectPreviousDistrict);
		_bindableButtonFactory.CreateAndBind(button2, NextDistrictKey, SelectNextDistrict);
		_tooltipRegistrar.Register(_districtHeader, _loc.T(ShowDistrictListKey));
		_tooltipRegistrar.RegisterWithKeyBinding(button, PreviousDistrictKey);
		_tooltipRegistrar.RegisterWithKeyBinding(button2, NextDistrictKey);
		_districtListPanel.Initialize(_root);
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_uiLayout.AddTopRight(_root, 9);
	}

	[OnEvent]
	public void OnEntityNameChanged(EntityNameChangedEvent entityNameChangedEvent)
	{
		if (entityNameChangedEvent.Entity.HasComponent<DistrictCenter>())
		{
			UpdateDistrictList();
		}
	}

	[OnEvent]
	public void OnDistrictSelected(DistrictSelectedEvent districtSelectedEvent)
	{
		SetDistrictNameButtonLabel();
		_districtHeader.AddToClassList(PanelDistrictClass);
	}

	[OnEvent]
	public void OnDistrictUnselected(DistrictUnselectedEvent districtUnselectedEvent)
	{
		ResetDistrictNameButtonLabel();
		_districtHeader.RemoveFromClassList(PanelDistrictClass);
	}

	public void Show()
	{
		_root.ToggleDisplayStyle(visible: true);
	}

	public void Hide()
	{
		_root.ToggleDisplayStyle(visible: false);
	}

	private void UpdateDistrictList()
	{
		if ((bool)_districtContextService.SelectedDistrict)
		{
			SetDistrictNameButtonLabel();
		}
		_districtListPanel.UpdateDistrictList();
	}

	private void ResetDistrictNameButtonLabel()
	{
		SetDistrictNameButtonLabel(_loc.T(GlobalViewLocKey));
	}

	private void ToggleDistrictSelection(ClickEvent evt)
	{
		if (_districtSelectionToggled)
		{
			_districtListPanel.Hide();
			_extensionToggler.AddToClassList(HiddenClass);
			_districtSelectionToggled = false;
		}
		else
		{
			_districtListPanel.Show();
			_extensionToggler.RemoveFromClassList(HiddenClass);
			_districtSelectionToggled = true;
		}
	}

	private void SetDistrictNameButtonLabel()
	{
		SetDistrictNameButtonLabel(_districtContextService.SelectedDistrict.DistrictName);
	}

	private void SetDistrictNameButtonLabel(string label)
	{
		_districtNameButton.text = label;
	}

	private void SelectPreviousDistrict()
	{
		SelectDistrict(IndexOfPreviousDistrict());
	}

	private int IndexOfPreviousDistrict()
	{
		if (!_districtContextService.SelectedDistrict)
		{
			return 0;
		}
		int num = IndexOfCurrentlySelectedDistrict - 1;
		if (num < 0)
		{
			num = DistrictCenters.Count - 1;
		}
		return num;
	}

	private void SelectNextDistrict()
	{
		SelectDistrict(IndexOfNextDistrict());
	}

	private int IndexOfNextDistrict()
	{
		if (!_districtContextService.SelectedDistrict)
		{
			return 0;
		}
		int num = IndexOfCurrentlySelectedDistrict + 1;
		if (num >= DistrictCenters.Count)
		{
			num = 0;
		}
		return num;
	}

	private void SelectDistrict(int index)
	{
		if (index < DistrictCenters.Count)
		{
			DistrictCenter target = DistrictCenters[index];
			_entitySelectionService.SelectAndFocusOn(target);
		}
	}
}

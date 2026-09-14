using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.Effects;
using Timberborn.EntityPanelSystem;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

internal class WellbeingFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly NeedViewFactory _needViewFactory;

	private readonly NeedGroupSpecService _needGroupSpecService;

	private readonly NeedGroupViewFactory _needGroupViewFactory;

	private readonly DevModeManager _devModeManager;

	private readonly WellbeingSummaryFactory _wellbeingSummaryFactory;

	private NeedManager _selectedNeedManager;

	private readonly Dictionary<string, NeedGroupView> _needGroupViews = new Dictionary<string, NeedGroupView>();

	private VisualElement _root;

	private VisualElement _needViewsElement;

	private VisualElement _wellbeingSummaryElement;

	private VisualElement _debugButtons;

	private Button _editButton;

	private Button _freezeButton;

	private Button _unfreezeButton;

	private Button _boostButton;

	private bool _editMode;

	private WellbeingSummary _wellbeingSummary;

	private bool InEditMode
	{
		get
		{
			if (_editMode)
			{
				return _devModeManager.Enabled;
			}
			return false;
		}
	}

	public WellbeingFragment(VisualElementLoader visualElementLoader, NeedViewFactory needViewFactory, NeedGroupSpecService needGroupSpecService, NeedGroupViewFactory needGroupViewFactory, DevModeManager devModeManager, WellbeingSummaryFactory wellbeingSummaryFactory)
	{
		_visualElementLoader = visualElementLoader;
		_needViewFactory = needViewFactory;
		_needGroupSpecService = needGroupSpecService;
		_needGroupViewFactory = needGroupViewFactory;
		_devModeManager = devModeManager;
		_wellbeingSummaryFactory = wellbeingSummaryFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/WellbeingFragment");
		_needViewsElement = _root.Q<VisualElement>("NeedViews");
		_wellbeingSummaryElement = _root.Q<VisualElement>("Summary");
		_debugButtons = _root.Q<VisualElement>("DebugButtons");
		_editButton = _root.Q<Button>("Edit");
		_editButton.RegisterCallback<ClickEvent>(delegate
		{
			_editMode = !_editMode;
		});
		_freezeButton = _root.Q<Button>("Freeze");
		_freezeButton.RegisterCallback<ClickEvent>(FreezeAllNeeds);
		_unfreezeButton = _root.Q<Button>("Unfreeze");
		_unfreezeButton.RegisterCallback<ClickEvent>(UnfreezeAllNeeds);
		_boostButton = _root.Q<Button>("Boost");
		_boostButton.RegisterCallback<ClickEvent>(BoostAllNeeds);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		NeedManager component = entity.GetComponent<NeedManager>();
		if ((bool)(BaseComponent)(object)component)
		{
			_selectedNeedManager = component;
			InitializeSummary();
			InitializeNeedGroups();
			InitializeNeeds();
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void ClearFragment()
	{
		ClearNeedViews();
		_wellbeingSummaryElement.Clear();
		_root.ToggleDisplayStyle(visible: false);
		_selectedNeedManager = null;
	}

	public void UpdateFragment()
	{
		if (!(BaseComponent)(object)_selectedNeedManager)
		{
			return;
		}
		_wellbeingSummary.UpdateContent();
		foreach (NeedGroupView value in _needGroupViews.Values)
		{
			value.Update(_selectedNeedManager, InEditMode);
		}
		_debugButtons.ToggleDisplayStyle(_devModeManager.Enabled);
	}

	private void InitializeSummary()
	{
		_wellbeingSummary = _wellbeingSummaryFactory.Create((BaseComponent)(object)_selectedNeedManager);
		_wellbeingSummaryElement.Add(_wellbeingSummary.Root);
	}

	private void InitializeNeedGroups()
	{
		foreach (NeedGroupSpec needGroup in _needGroupSpecService.NeedGroups)
		{
			NeedGroupView needGroupView = _needGroupViewFactory.Create(needGroup);
			_needViewsElement.Add(needGroupView.Root);
			_needGroupViews.Add(needGroup.Id, needGroupView);
		}
	}

	private void InitializeNeeds()
	{
		foreach (NeedSpec needSpec in _selectedNeedManager.NeedSpecs)
		{
			NeedView needView = _needViewFactory.Create(needSpec, _selectedNeedManager);
			_needGroupViews[needSpec.NeedGroupId].AddNeed(needView);
		}
		foreach (NeedGroupView value in _needGroupViews.Values)
		{
			value.UpdateVisibility();
		}
	}

	private void ClearNeedViews()
	{
		foreach (NeedGroupView value in _needGroupViews.Values)
		{
			value.ClearNeedViews();
		}
		_needViewsElement.Clear();
		_needGroupViews.Clear();
	}

	private void FreezeAllNeeds(ClickEvent evt)
	{
		if ((bool)(BaseComponent)(object)_selectedNeedManager)
		{
			foreach (NeedSpec needSpec in _selectedNeedManager.NeedSpecs)
			{
				_selectedNeedManager.DisableUpdate(needSpec.Id);
			}
		}
	}

	private void UnfreezeAllNeeds(ClickEvent evt)
	{
		if ((bool)(BaseComponent)(object)_selectedNeedManager)
		{
			foreach (NeedSpec needSpec in _selectedNeedManager.NeedSpecs)
			{
				_selectedNeedManager.EnableUpdate(needSpec.Id);
			}
		}
	}

	private void BoostAllNeeds(ClickEvent evt)
	{
		if ((bool)(BaseComponent)(object)_selectedNeedManager)
		{
			foreach (NeedSpec needSpec in _selectedNeedManager.NeedSpecs)
			{
				_selectedNeedManager.ApplyEffect(new InstantEffect(needSpec.Id, _selectedNeedManager.NeedPointsToMax(needSpec.Id), 1));
			}
		}
	}
}

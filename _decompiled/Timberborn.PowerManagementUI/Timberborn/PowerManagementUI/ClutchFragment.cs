using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.PowerManagement;
using UnityEngine.UIElements;

namespace Timberborn.PowerManagementUI;

internal class ClutchFragment : IEntityPanelFragment
{
	private static readonly string EngagedLocKey = "Building.Clutch.Mode.Engaged";

	private static readonly string DisengagedLocKey = "Building.Clutch.Mode.Disengaged";

	private static readonly string AutomatedLocKey = "Automation.Mode.Automated";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly ClutchModeToggleFactory _clutchModeToggleFactory;

	private VisualElement _root;

	private ClutchModeToggle _modeToggle;

	private Label _modeLabel;

	private string _automatedEngagedText;

	private string _automatedDisengagedText;

	private Clutch _clutch;

	public ClutchFragment(VisualElementLoader visualElementLoader, ILoc loc, ClutchModeToggleFactory clutchModeToggleFactory)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_clutchModeToggleFactory = clutchModeToggleFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/ClutchFragment");
		_modeToggle = _clutchModeToggleFactory.Create(_root.Q<VisualElement>("ModeToggle"));
		_modeLabel = _root.Q<Label>("ModeLabel");
		_automatedEngagedText = BuildAutomatedStateText(EngagedLocKey);
		_automatedDisengagedText = BuildAutomatedStateText(DisengagedLocKey);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_clutch = entity.GetComponent<Clutch>();
		if ((bool)_clutch)
		{
			_modeToggle.Show(_clutch);
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void ClearFragment()
	{
		_clutch = null;
		_modeToggle.Clear();
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_clutch)
		{
			UpdateModeToggle();
		}
	}

	private string BuildAutomatedStateText(string stateLocKey)
	{
		return _loc.T(AutomatedLocKey) + " (" + _loc.T(stateLocKey) + ")";
	}

	private void UpdateModeToggle()
	{
		_modeToggle.Update();
		Label modeLabel = _modeLabel;
		modeLabel.text = _clutch.Mode switch
		{
			ClutchMode.Engaged => _loc.T(EngagedLocKey), 
			ClutchMode.Disengaged => _loc.T(DisengagedLocKey), 
			ClutchMode.Automated => _clutch.IsEngaged ? _automatedEngagedText : _automatedDisengagedText, 
			_ => throw new ArgumentException($"Unexpected mode: {_clutch.Mode}"), 
		};
	}
}

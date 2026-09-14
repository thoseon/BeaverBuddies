using Timberborn.AutomationBuildings;
using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class GateToggle
{
	private static readonly string OpenedClass = "gate-toggle__icon--open";

	private static readonly string ClosedClass = "gate-toggle__icon--closed";

	private static readonly string AutomatedClass = "gate-toggle__icon--automated";

	private static readonly string ToggleOpenLocKey = "Toggle.State.Open";

	private static readonly string ToggleClosedLocKey = "Toggle.State.Closed";

	private static readonly string ToggleConflictLocKey = "Toggle.State.Conflict";

	private static readonly string ToggleAutomatedLocKey = "Automation.Mode.Automated";

	private readonly SliderToggleFactory _sliderToggleFactory;

	private readonly ILoc _loc;

	private Gate _gate;

	private SliderToggle _sliderToggle;

	private Label _modeLabel;

	private string _automatedOpenText;

	private string _automatedClosedText;

	private string _automatedConflictText;

	public GateToggle(SliderToggleFactory sliderToggleFactory, ILoc loc)
	{
		_sliderToggleFactory = sliderToggleFactory;
		_loc = loc;
	}

	public void Initialize(VisualElement parent, Label modeLabel)
	{
		SliderToggleItem sliderToggleItem = SliderToggleItem.Create(() => _loc.T(ToggleOpenLocKey), OpenedClass, delegate
		{
			_gate.Open();
		}, () => _gate.OpenMode);
		SliderToggleItem sliderToggleItem2 = SliderToggleItem.Create(() => _loc.T(ToggleClosedLocKey), ClosedClass, delegate
		{
			_gate.Close();
		}, () => _gate.ClosedMode);
		SliderToggleItem sliderToggleItem3 = SliderToggleItem.Create(() => _loc.T(ToggleAutomatedLocKey), AutomatedClass, delegate
		{
			_gate.Automate();
		}, () => _gate.AutomatedMode);
		_sliderToggle = _sliderToggleFactory.Create(parent, sliderToggleItem, sliderToggleItem2, sliderToggleItem3);
		_modeLabel = modeLabel;
		_automatedOpenText = _loc.T(ToggleAutomatedLocKey) + " (" + _loc.T(ToggleOpenLocKey) + ")";
		_automatedClosedText = _loc.T(ToggleAutomatedLocKey) + " (" + _loc.T(ToggleClosedLocKey) + ")";
		_automatedConflictText = _loc.T(ToggleAutomatedLocKey) + " (" + _loc.T(ToggleConflictLocKey) + ")";
	}

	public void Show(Gate gate)
	{
		_gate = gate;
	}

	public void Update()
	{
		if (_gate != null)
		{
			_sliderToggle.Update();
			_modeLabel.text = GetModeLabel();
		}
	}

	public void Clear()
	{
		_gate = null;
	}

	private string GetModeLabel()
	{
		if (_gate.OpenMode)
		{
			return _loc.T(ToggleOpenLocKey);
		}
		if (_gate.ClosedMode)
		{
			return _loc.T(ToggleClosedLocKey);
		}
		if (_gate.AutomatedMode)
		{
			if (!_gate.IsConflict)
			{
				if (!_gate.IsOpenByAutomation)
				{
					return _automatedClosedText;
				}
				return _automatedOpenText;
			}
			return _automatedConflictText;
		}
		return string.Empty;
	}
}

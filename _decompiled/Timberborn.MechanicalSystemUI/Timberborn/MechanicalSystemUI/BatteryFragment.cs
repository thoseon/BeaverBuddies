using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.MechanicalSystem;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.MechanicalSystemUI;

internal class BatteryFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly DevModeManager _devModeManager;

	private VisualElement _root;

	private Label _chargeLabel;

	private Timberborn.CoreUI.ProgressBar _progressBar;

	private Slider _chargeSlider;

	private MechanicalNode _mechanicalNode;

	private readonly Phrase _chargePhrase = Phrase.New("Mechanical.BatteryCharge").Format((int value) => value.ToString()).FormatPowerCapacity<int>();

	public BatteryFragment(VisualElementLoader visualElementLoader, ILoc loc, DevModeManager devModeManager)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_devModeManager = devModeManager;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/BatteryFragment");
		_chargeLabel = _root.Q<Label>("Charge");
		_progressBar = _root.Q<Timberborn.CoreUI.ProgressBar>("ProgressBar");
		_chargeSlider = _root.Q<Slider>("ChargeSlider");
		_chargeSlider.lowValue = 0f;
		_chargeSlider.highValue = 1f;
		_chargeSlider.RegisterValueChangedCallback(ChangeCharge);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_mechanicalNode = entity.GetComponent<MechanicalNode>();
	}

	public void ClearFragment()
	{
		_root.ToggleDisplayStyle(visible: false);
		_mechanicalNode = null;
	}

	public void UpdateFragment()
	{
		MechanicalNode mechanicalNode = _mechanicalNode;
		if (mechanicalNode != null && mechanicalNode.IsBattery && mechanicalNode.Active)
		{
			_chargeLabel.text = _loc.T(_chargePhrase, _mechanicalNode.NominalBatteryCharge, _mechanicalNode.NominalBatteryCapacity);
			_progressBar.SetProgress(_mechanicalNode.NominalBatteryChargeLevel);
			_root.ToggleDisplayStyle(visible: true);
			if (_devModeManager.Enabled)
			{
				_chargeSlider.SetValueWithoutNotify(_mechanicalNode.NominalBatteryChargeLevel);
				_chargeSlider.ToggleDisplayStyle(visible: true);
			}
			else
			{
				_chargeSlider.ToggleDisplayStyle(visible: false);
			}
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void ChangeCharge(ChangeEvent<float> changeEvent)
	{
		_mechanicalNode.Battery.ModifyCharge(float.MinValue);
		_mechanicalNode.Battery.ModifyCharge(changeEvent.newValue * (float)_mechanicalNode.NominalBatteryCapacity);
	}
}

using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using Timberborn.WaterBuildings;
using UnityEngine.UIElements;

namespace Timberborn.WaterBuildingsUI;

internal class WaterMoverFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly WaterMoverToggleFactory _waterMoverToggleFactory;

	private readonly ILoc _loc;

	private VisualElement _root;

	private WaterMover _waterMover;

	private WaterMoverToggle _waterMoverToggle;

	private Label _flowRateLabel;

	private PreciseSlider _flowRateSlider;

	private readonly Phrase _flowRatePhrase = Phrase.New("Buildings.MechanicalPump.FlowRate").FormatFlow<float>("F2");

	public WaterMoverFragment(VisualElementLoader visualElementLoader, WaterMoverToggleFactory waterMoverToggleFactory, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_waterMoverToggleFactory = waterMoverToggleFactory;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/WaterMoverFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_waterMoverToggle = _waterMoverToggleFactory.Create(_root.Q("Toggle"));
		_flowRateLabel = _root.Q<Label>("EfficiencyLabel");
		_flowRateSlider = _root.Q<PreciseSlider>("Efficiency");
		_flowRateSlider.SetValueChangedCallback(SetFlowRate);
		_flowRateSlider.SetStepWithoutNotify(0.01f);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		WaterMover component = entity.GetComponent<WaterMover>();
		if (component != null)
		{
			_waterMover = component;
			_waterMoverToggle.Show(component);
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void ClearFragment()
	{
		_waterMover = null;
		_waterMoverToggle.Clear();
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_waterMover)
		{
			_flowRateLabel.text = _loc.T(_flowRatePhrase, _waterMover.FlowRate);
			_flowRateSlider.UpdateValuesWithoutNotify(_waterMover.FlowRate, _waterMover.MaxFlowRate);
			_flowRateSlider.SetMarker(_waterMover.EffectiveFlowRate);
			_waterMoverToggle.Update();
		}
	}

	private void SetFlowRate(float value)
	{
		_waterMover.SetFlowRate(value);
	}
}

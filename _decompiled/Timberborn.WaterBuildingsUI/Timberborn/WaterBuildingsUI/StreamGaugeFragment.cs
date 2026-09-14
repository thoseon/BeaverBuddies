using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using Timberborn.UIFormatters;
using Timberborn.WaterBuildings;
using UnityEngine.UIElements;

namespace Timberborn.WaterBuildingsUI;

internal class StreamGaugeFragment : IEntityPanelFragment, IInputProcessor
{
	private static readonly string UniqueBuildingActionKey = "UniqueBuildingAction";

	private static readonly string ResetGreatestDepthLocKey = "Building.StreamGauge.Reset";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly InputService _inputService;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private Label _depthLabel;

	private Label _greatestDepthLabel;

	private Label _currentLabel;

	private Label _contaminationLevelLabel;

	private StreamGauge _streamGauge;

	private VisualElement _root;

	private readonly Phrase _depthPhrase = Phrase.New("Building.StreamGauge.Depth").FormatDistance<float>("F2");

	private readonly Phrase _greatestDepthPhrase = Phrase.New("Building.StreamGauge.GreatestDepth").FormatDistance<float>("F2");

	private readonly Phrase _currentPhrase = Phrase.New("Building.StreamGauge.Current").FormatFlow<float>("F1");

	private readonly Phrase _contaminationLevelPhrase = Phrase.New("Building.StreamGauge.Contamination").FormatPercentRounded();

	public StreamGaugeFragment(VisualElementLoader visualElementLoader, ILoc loc, InputService inputService, ITooltipRegistrar tooltipRegistrar)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_inputService = inputService;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/StreamGaugeFragment");
		Button button = _root.Q<Button>("ResetGreatestDepthButton");
		button.RegisterCallback<ClickEvent>(delegate
		{
			_streamGauge.ResetHighestWaterLevel();
		});
		_tooltipRegistrar.RegisterWithKeyBinding(button, _loc.T(ResetGreatestDepthLocKey), UniqueBuildingActionKey);
		_root.ToggleDisplayStyle(visible: false);
		_depthLabel = _root.Q<Label>("DepthLabel");
		_greatestDepthLabel = _root.Q<Label>("GreatestDepthLabel");
		_currentLabel = _root.Q<Label>("CurrentLabel");
		_contaminationLevelLabel = _root.Q<Label>("ContaminationLabel");
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_streamGauge = entity.GetComponent<StreamGauge>();
		if (_streamGauge != null)
		{
			_inputService.AddInputProcessor(this);
		}
	}

	public void ClearFragment()
	{
		_streamGauge = null;
		_root.ToggleDisplayStyle(visible: false);
		_inputService.RemoveInputProcessor(this);
	}

	public void UpdateFragment()
	{
		if ((bool)_streamGauge && _streamGauge.Enabled)
		{
			_root.ToggleDisplayStyle(visible: true);
			_depthLabel.text = _loc.T(_depthPhrase, _streamGauge.WaterLevel);
			_greatestDepthLabel.text = _loc.T(_greatestDepthPhrase, _streamGauge.HighestWaterLevel);
			_currentLabel.text = _loc.T(_currentPhrase, _streamGauge.WaterCurrent);
			_contaminationLevelLabel.text = _loc.T(_contaminationLevelPhrase, _streamGauge.ContaminationLevel);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(UniqueBuildingActionKey))
		{
			_streamGauge.ResetHighestWaterLevel();
			return true;
		}
		return false;
	}
}

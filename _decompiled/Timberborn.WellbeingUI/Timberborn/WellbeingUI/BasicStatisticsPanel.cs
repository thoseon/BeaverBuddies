using Timberborn.BatchControl;
using Timberborn.GameDistricts;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.Population;
using Timberborn.ScienceSystem;
using Timberborn.SingletonSystem;
using Timberborn.UIFormatters;
using Timberborn.UILayoutSystem;
using Timberborn.Wellbeing;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

public class BasicStatisticsPanel : ILoadableSingleton, IUpdatableSingleton, IInputProcessor
{
	private static readonly string BeaversPerishedClass = "basic-statistics__bot";

	private static readonly string AllPerishedClass = "basic-statistics__death";

	private static readonly string NegativeWellbeingClass = "basic-statistics__negative";

	private static readonly string HighlightClass = "highlight";

	private static readonly string ToggleBatchControlBoxKey = "ToggleBatchControlBox";

	private readonly UILayout _uiLayout;

	private readonly WellbeingService _wellbeingService;

	private readonly DistrictContextService _districtContextService;

	private readonly ScienceService _scienceService;

	private readonly IBatchControlBox _batchControlBox;

	private readonly InputService _inputService;

	private readonly PopulationService _populationService;

	private readonly BasicStatisticsPanelFactory _basicStatisticsPanelFactory;

	private readonly EventBus _eventBus;

	private readonly ILoc _loc;

	private Button _wellbeingButton;

	private Label _wellbeingCount;

	private Label _scienceCount;

	private VisualElement _root;

	private readonly Phrase _sciencePhrase = Phrase.New().FormatCompact();

	public BasicStatisticsPanel(UILayout uiLayout, WellbeingService wellbeingService, DistrictContextService districtContextService, ScienceService scienceService, IBatchControlBox batchControlBox, InputService inputService, PopulationService populationService, BasicStatisticsPanelFactory basicStatisticsPanelFactory, EventBus eventBus, ILoc loc)
	{
		_uiLayout = uiLayout;
		_wellbeingService = wellbeingService;
		_districtContextService = districtContextService;
		_scienceService = scienceService;
		_batchControlBox = batchControlBox;
		_inputService = inputService;
		_populationService = populationService;
		_basicStatisticsPanelFactory = basicStatisticsPanelFactory;
		_eventBus = eventBus;
		_loc = loc;
	}

	public void Load()
	{
		_root = _basicStatisticsPanelFactory.Create();
		_wellbeingButton = _root.Q<Button>("Wellbeing");
		_wellbeingCount = _root.Q<Label>("WellbeingCount");
		_scienceCount = _root.Q<Label>("ScienceCount");
		_eventBus.Register(this);
	}

	public void UpdateSingleton()
	{
		UpdateWellbeing();
		UpdateScience();
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(ToggleBatchControlBoxKey))
		{
			_batchControlBox.OpenBatchControlBox();
			return true;
		}
		return false;
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_inputService.AddInputProcessor(this);
		_uiLayout.AddTopLeft(_root, 1);
	}

	public void ToggleWellbeingButtonHighlight(bool state)
	{
		_wellbeingButton.EnableInClassList(HighlightClass, state);
	}

	private void UpdateWellbeing()
	{
		_wellbeingButton.RemoveFromClassList(BeaversPerishedClass);
		_wellbeingButton.RemoveFromClassList(AllPerishedClass);
		if (_populationService.OnlyBotsAlive)
		{
			_wellbeingCount.text = "";
			_wellbeingButton.AddToClassList(BeaversPerishedClass);
			_wellbeingButton.RemoveFromClassList(NegativeWellbeingClass);
		}
		else if (_populationService.AllDead)
		{
			_wellbeingCount.text = "";
			_wellbeingButton.AddToClassList(AllPerishedClass);
			_wellbeingButton.AddToClassList(NegativeWellbeingClass);
		}
		else
		{
			int num = (_districtContextService.SelectedDistrict ? _wellbeingService.AverageDistrictWellbeing : _wellbeingService.AverageGlobalWellbeing);
			_wellbeingCount.text = num.ToString();
			_wellbeingButton.EnableInClassList(NegativeWellbeingClass, num < 0);
		}
	}

	private void UpdateScience()
	{
		_scienceCount.text = _loc.T(_sciencePhrase, _scienceService.SciencePoints);
	}
}

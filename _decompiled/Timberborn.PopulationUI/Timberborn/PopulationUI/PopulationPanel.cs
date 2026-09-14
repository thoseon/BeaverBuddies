using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.GameDistricts;
using Timberborn.Population;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.PopulationUI;

internal class PopulationPanel : ILoadableSingleton
{
	private static readonly string PanelDistrictClass = "panel--district";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly UILayout _uiLayout;

	private readonly PopulationService _populationService;

	private readonly DistrictContextService _districtContextService;

	private readonly EventBus _eventBus;

	private readonly IBatchControlBox _batchControlBox;

	private readonly PopulationDataRowFactory _populationDataRowFactory;

	private readonly HousingDataRowFactory _housingDataRowFactory;

	private readonly WorkplaceDataRowFactory _workplaceDataRowFactory;

	private VisualElement _root;

	private PopulationDataRow _populationDataRow;

	private HousingDataRow _housingDataRow;

	private WorkplaceDataRow _beaverWorkplaceData;

	private WorkplaceDataRow _botWorkplaceData;

	private Button _botWorkplaceDataButton;

	public PopulationPanel(VisualElementLoader visualElementLoader, UILayout uiLayout, PopulationService populationService, DistrictContextService districtContextService, EventBus eventBus, IBatchControlBox batchControlBox, PopulationDataRowFactory populationDataRowFactory, HousingDataRowFactory housingDataRowFactory, WorkplaceDataRowFactory workplaceDataRowFactory)
	{
		_visualElementLoader = visualElementLoader;
		_uiLayout = uiLayout;
		_populationService = populationService;
		_districtContextService = districtContextService;
		_eventBus = eventBus;
		_batchControlBox = batchControlBox;
		_populationDataRowFactory = populationDataRowFactory;
		_housingDataRowFactory = housingDataRowFactory;
		_workplaceDataRowFactory = workplaceDataRowFactory;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/Population/PopulationPanel");
		AddPopulationRow();
		AddHousingRow();
		AddBeaverWorkplaceRow();
		AddBotWorkplaceRow();
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_uiLayout.AddTopLeft(_root, 2);
	}

	[OnEvent]
	public void OnPopulationChangedEvent(PopulationChangedEvent populationChangedEvent)
	{
		UpdateCounters();
	}

	[OnEvent]
	public void OnDistrictSelected(DistrictSelectedEvent districtSelectedEvent)
	{
		UpdateCounters();
	}

	[OnEvent]
	public void OnDistrictUnselected(DistrictUnselectedEvent districtUnselectedEvent)
	{
		UpdateCounters();
	}

	private void AddPopulationRow()
	{
		Button button = _root.Q<Button>("PopulationData");
		_populationDataRow = _populationDataRowFactory.Create(button, GetContextualPopulationData);
		button.RegisterCallback<ClickEvent>(delegate
		{
			_batchControlBox.OpenCharactersTab();
		});
	}

	private void AddHousingRow()
	{
		Button button = _root.Q<Button>("HousingData");
		_housingDataRow = _housingDataRowFactory.Create(button, GetContextualPopulationData);
		button.RegisterCallback<ClickEvent>(delegate
		{
			_batchControlBox.OpenHousingTab();
		});
	}

	private void AddBeaverWorkplaceRow()
	{
		Button button = _root.Q<Button>("BeaverWorkplaceData");
		_beaverWorkplaceData = _workplaceDataRowFactory.CreateBeaverWorkplaceDataRow(button, GetContextualPopulationData);
		button.RegisterCallback<ClickEvent>(delegate
		{
			_batchControlBox.OpenWorkplacesTab();
		});
	}

	private void AddBotWorkplaceRow()
	{
		_botWorkplaceDataButton = _root.Q<Button>("BotWorkplaceData");
		_botWorkplaceData = _workplaceDataRowFactory.CreateBotWorkplaceDataRow(_botWorkplaceDataButton, GetContextualPopulationData);
		_botWorkplaceDataButton.RegisterCallback<ClickEvent>(delegate
		{
			_batchControlBox.OpenWorkplacesTab();
		});
	}

	private PopulationData GetContextualPopulationData()
	{
		if (!_districtContextService.SelectedDistrict)
		{
			return _populationService.GlobalPopulationData;
		}
		return _populationService.DistrictPopulationData;
	}

	private void UpdateCounters()
	{
		_populationDataRow.UpdateData();
		_housingDataRow.UpdateData();
		_beaverWorkplaceData.UpdateData();
		_botWorkplaceData.UpdateData();
		bool botCreated = _populationService.BotCreated;
		_botWorkplaceDataButton.ToggleDisplayStyle(botCreated);
		_root.EnableInClassList(PanelDistrictClass, _districtContextService.SelectedDistrict);
	}
}

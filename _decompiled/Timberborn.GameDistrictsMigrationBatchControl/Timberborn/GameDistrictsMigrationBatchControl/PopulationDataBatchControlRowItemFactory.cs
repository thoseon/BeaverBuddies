using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.GameDistricts;
using Timberborn.Population;
using Timberborn.PopulationUI;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsMigrationBatchControl;

internal class PopulationDataBatchControlRowItemFactory
{
	private readonly HousingDataRowFactory _housingDataRowFactory;

	private readonly PopulationDataCollector _populationDataCollector;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly WorkplaceDataRowFactory _workplaceDataRowFactory;

	public PopulationDataBatchControlRowItemFactory(HousingDataRowFactory housingDataRowFactory, PopulationDataCollector populationDataCollector, VisualElementLoader visualElementLoader, WorkplaceDataRowFactory workplaceDataRowFactory)
	{
		_housingDataRowFactory = housingDataRowFactory;
		_populationDataCollector = populationDataCollector;
		_visualElementLoader = visualElementLoader;
		_workplaceDataRowFactory = workplaceDataRowFactory;
	}

	public IBatchControlRowItem CreateHousingDataRowItem(DistrictCenter districtCenter)
	{
		VisualElement visualElement = CreateRoot();
		PopulationData populationData = CreatePopulationData(districtCenter);
		HousingDataRow housingDataRow = _housingDataRowFactory.Create(visualElement.Q<VisualElement>("Content"), () => populationData);
		housingDataRow.UpdateData();
		return new PopulationDataBatchControlRowItem(_populationDataCollector, visualElement, districtCenter, populationData, housingDataRow);
	}

	public IBatchControlRowItem CreateBeaverWorkplaceRowItem(DistrictCenter districtCenter)
	{
		VisualElement visualElement = CreateRoot();
		PopulationData populationData = CreatePopulationData(districtCenter);
		WorkplaceDataRow workplaceDataRow = _workplaceDataRowFactory.CreateBeaverWorkplaceDataRow(visualElement.Q<VisualElement>("Content"), () => populationData);
		workplaceDataRow.UpdateData();
		return new PopulationDataBatchControlRowItem(_populationDataCollector, visualElement, districtCenter, populationData, workplaceDataRow);
	}

	public IBatchControlRowItem CreateBotWorkplaceRowItem(DistrictCenter districtCenter)
	{
		VisualElement visualElement = CreateRoot();
		PopulationData populationData = CreatePopulationData(districtCenter);
		WorkplaceDataRow workplaceDataRow = _workplaceDataRowFactory.CreateBotWorkplaceDataRow(visualElement.Q<VisualElement>("Content"), () => populationData);
		workplaceDataRow.UpdateData();
		return new PopulationDataBatchControlRowItem(_populationDataCollector, visualElement, districtCenter, populationData, workplaceDataRow);
	}

	private VisualElement CreateRoot()
	{
		string elementName = "Game/BatchControl/PopulationDataBatchControlRowItem";
		return _visualElementLoader.LoadVisualElement(elementName);
	}

	private PopulationData CreatePopulationData(DistrictCenter districtCenter)
	{
		PopulationData populationData = new PopulationData();
		_populationDataCollector.CollectData(districtCenter, populationData);
		return populationData;
	}
}

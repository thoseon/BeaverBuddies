using Timberborn.BatchControl;
using Timberborn.GameDistricts;
using Timberborn.Population;
using Timberborn.PopulationUI;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsMigrationBatchControl;

internal class PopulationDataBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private readonly PopulationDataCollector _populationDataCollector;

	private readonly DistrictCenter _districtCenter;

	private readonly PopulationData _populationData;

	private readonly IPopulationRow _populationRow;

	public VisualElement Root { get; }

	public PopulationDataBatchControlRowItem(PopulationDataCollector populationDataCollector, VisualElement root, DistrictCenter districtCenter, PopulationData populationData, IPopulationRow populationRow)
	{
		_populationDataCollector = populationDataCollector;
		Root = root;
		_districtCenter = districtCenter;
		_populationData = populationData;
		_populationRow = populationRow;
	}

	public void UpdateRowItem()
	{
		if (_populationDataCollector.CollectData(_districtCenter, _populationData))
		{
			_populationRow.UpdateData();
		}
	}
}

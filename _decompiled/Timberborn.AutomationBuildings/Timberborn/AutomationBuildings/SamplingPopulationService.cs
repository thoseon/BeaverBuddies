using System.Collections.Generic;
using Timberborn.Automation;
using Timberborn.Common;
using Timberborn.GameDistricts;
using Timberborn.Population;

namespace Timberborn.AutomationBuildings;

internal class SamplingPopulationService : ISamplingSingleton
{
	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly PopulationDataCollector _populationDataCollector;

	private readonly PopulationService _populationService;

	private readonly Dictionary<DistrictCenter, PopulationData> _districtPopulationData = new Dictionary<DistrictCenter, PopulationData>();

	private readonly Dictionary<DistrictCenter, PopulationData> _oldDistrictPopulationData = new Dictionary<DistrictCenter, PopulationData>();

	private readonly PopulationData _emptyPopulationData = new PopulationData();

	public PopulationData GlobalPopulationData { get; } = new PopulationData();

	public SamplingPopulationService(DistrictCenterRegistry districtCenterRegistry, PopulationDataCollector populationDataCollector, PopulationService populationService)
	{
		_districtCenterRegistry = districtCenterRegistry;
		_populationDataCollector = populationDataCollector;
		_populationService = populationService;
	}

	public void Sample()
	{
		GlobalPopulationData.CopyFrom(_populationService.GlobalPopulationData);
		_oldDistrictPopulationData.AddRange(_districtPopulationData);
		_districtPopulationData.Clear();
		ReadOnlyList<DistrictCenter> finishedDistrictCenters = _districtCenterRegistry.FinishedDistrictCenters;
		for (int i = 0; i < finishedDistrictCenters.Count; i++)
		{
			DistrictCenter districtCenter = finishedDistrictCenters[i];
			PopulationData populationData = (_oldDistrictPopulationData.TryGetValue(districtCenter, out var value) ? value : new PopulationData());
			_populationDataCollector.CollectData(districtCenter, populationData);
			_districtPopulationData.Add(districtCenter, populationData);
		}
		_oldDistrictPopulationData.Clear();
	}

	public PopulationData GetDistrictData(DistrictCenter districtCenter)
	{
		return _districtPopulationData.GetValueOrDefault(districtCenter, _emptyPopulationData);
	}
}

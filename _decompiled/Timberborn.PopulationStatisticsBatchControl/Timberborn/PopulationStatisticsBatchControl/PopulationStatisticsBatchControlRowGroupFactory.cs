using Timberborn.BatchControl;
using Timberborn.GameDistricts;
using Timberborn.PopulationStatisticsSampling;
using UnityEngine.UIElements;

namespace Timberborn.PopulationStatisticsBatchControl;

internal class PopulationStatisticsBatchControlRowGroupFactory
{
	private readonly BatchControlRowGroupFactory _batchControlRowGroupFactory;

	private readonly PopulationStatisticsRowItemFactory _populationStatisticsRowItemFactory;

	public PopulationStatisticsBatchControlRowGroupFactory(BatchControlRowGroupFactory batchControlRowGroupFactory, PopulationStatisticsRowItemFactory populationStatisticsRowItemFactory)
	{
		_batchControlRowGroupFactory = batchControlRowGroupFactory;
		_populationStatisticsRowItemFactory = populationStatisticsRowItemFactory;
	}

	public BatchControlRowGroup Create(DistrictCenter districtCenter)
	{
		DistrictPopulationSamplesRegistry component = districtCenter.GetComponent<DistrictPopulationSamplesRegistry>();
		BatchControlRowGroup batchControlRowGroup = _batchControlRowGroupFactory.CreateUnsorted(new BatchControlRow(new VisualElement()));
		batchControlRowGroup.AddRow(_populationStatisticsRowItemFactory.Create(component));
		return batchControlRowGroup;
	}

	public BatchControlRowGroup CreateGlobal()
	{
		BatchControlRowGroup batchControlRowGroup = _batchControlRowGroupFactory.CreateUnsorted(new BatchControlRow(new VisualElement()));
		batchControlRowGroup.AddRow(_populationStatisticsRowItemFactory.CreateGlobal());
		return batchControlRowGroup;
	}
}

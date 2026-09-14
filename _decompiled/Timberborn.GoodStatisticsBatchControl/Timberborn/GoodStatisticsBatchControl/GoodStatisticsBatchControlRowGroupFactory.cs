using Timberborn.BatchControl;
using Timberborn.GameDistricts;
using Timberborn.GoodsSampling;
using UnityEngine.UIElements;

namespace Timberborn.GoodStatisticsBatchControl;

internal class GoodStatisticsBatchControlRowGroupFactory
{
	private readonly BatchControlRowGroupFactory _batchControlRowGroupFactory;

	private readonly GoodStatisticsRowItemFactory _goodStatisticsRowItemFactory;

	public GoodStatisticsBatchControlRowGroupFactory(BatchControlRowGroupFactory batchControlRowGroupFactory, GoodStatisticsRowItemFactory goodStatisticsRowItemFactory)
	{
		_batchControlRowGroupFactory = batchControlRowGroupFactory;
		_goodStatisticsRowItemFactory = goodStatisticsRowItemFactory;
	}

	public BatchControlRowGroup Create(DistrictCenter districtCenter)
	{
		DistrictGoodSamplingRegistry component = districtCenter.GetComponent<DistrictGoodSamplingRegistry>();
		BatchControlRowGroup batchControlRowGroup = _batchControlRowGroupFactory.CreateUnsorted(new BatchControlRow(new VisualElement()));
		batchControlRowGroup.AddRow(_goodStatisticsRowItemFactory.Create(component));
		return batchControlRowGroup;
	}

	public BatchControlRowGroup CreateGlobal()
	{
		BatchControlRowGroup batchControlRowGroup = _batchControlRowGroupFactory.CreateUnsorted(new BatchControlRow(new VisualElement()));
		batchControlRowGroup.AddRow(_goodStatisticsRowItemFactory.CreateGlobal());
		return batchControlRowGroup;
	}
}

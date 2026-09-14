using Timberborn.BatchControl;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.GoodsSampling;

namespace Timberborn.GoodStatisticsBatchControl;

internal class GoodStatisticsRowItemFactory
{
	private readonly GoodStatisticsGroupFactory _goodStatisticsGroupFactory;

	private readonly GoodsGroupSpecService _goodsGroupSpecService;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly BatchControlDistrict _batchControlDistrict;

	private readonly GlobalGoodSamplingRegistry _globalGoodSamplingRegistry;

	private ReadOnlyList<GoodGroupSpec> GoodGroupSpecifications => _goodsGroupSpecService.GoodGroupSpecs;

	public GoodStatisticsRowItemFactory(GoodStatisticsGroupFactory goodStatisticsGroupFactory, GoodsGroupSpecService goodsGroupSpecService, VisualElementLoader visualElementLoader, BatchControlDistrict batchControlDistrict, GlobalGoodSamplingRegistry globalGoodSamplingRegistry)
	{
		_goodStatisticsGroupFactory = goodStatisticsGroupFactory;
		_goodsGroupSpecService = goodsGroupSpecService;
		_visualElementLoader = visualElementLoader;
		_batchControlDistrict = batchControlDistrict;
		_globalGoodSamplingRegistry = globalGoodSamplingRegistry;
	}

	public BatchControlRow Create(DistrictGoodSamplingRegistry districtGoodSamplingRegistry)
	{
		string elementName = "Game/BatchControl/GoodStatisticsRowItem";
		return new BatchControlRow(_visualElementLoader.LoadVisualElement(elementName), districtGoodSamplingRegistry.GetComponent<EntityComponent>(), () => _batchControlDistrict.SelectedDistrict, CreateGoodGroups(districtGoodSamplingRegistry.GoodSamplingRegistry));
	}

	public BatchControlRow CreateGlobal()
	{
		string elementName = "Game/BatchControl/GoodStatisticsRowItem";
		return new BatchControlRow(_visualElementLoader.LoadVisualElement(elementName), null, () => !_batchControlDistrict.SelectedDistrict, CreateGoodGroups(_globalGoodSamplingRegistry.GoodSamplingRegistry));
	}

	private IBatchControlRowItem[] CreateGoodGroups(GoodSamplingRegistry goodSamplingRegistry)
	{
		IBatchControlRowItem[] array = new IBatchControlRowItem[GoodGroupSpecifications.Count];
		for (int i = 0; i < GoodGroupSpecifications.Count; i++)
		{
			GoodGroupSpec goodGroupSpec = GoodGroupSpecifications[i];
			array[i] = _goodStatisticsGroupFactory.Create(goodGroupSpec, goodSamplingRegistry);
		}
		return array;
	}
}

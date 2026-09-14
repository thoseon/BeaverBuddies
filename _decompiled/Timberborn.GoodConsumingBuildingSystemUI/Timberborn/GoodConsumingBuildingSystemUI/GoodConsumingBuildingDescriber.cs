using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.GoodConsumingBuildingSystem;
using Timberborn.GoodsUI;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using Timberborn.WorkSystem;
using UnityEngine.UIElements;

namespace Timberborn.GoodConsumingBuildingSystemUI;

internal class GoodConsumingBuildingDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string DescriptionLocKey = "GoodConsuming.SupplyDescription";

	private static readonly string NeedsHaulersLocKey = "GoodConsuming.NeedsHaulers";

	private readonly GoodDescriber _goodDescriber;

	private readonly ResourceAmountFormatter _resourceAmountFormatter;

	private readonly ILoc _loc;

	private readonly DescribedAmountFactory _describedAmountFactory;

	private readonly ProductionItemFactory _productionItemFactory;

	private GoodConsumingBuilding _goodConsumingBuilding;

	private BlockObject _blockObject;

	private Workplace _workplace;

	private string _time;

	public GoodConsumingBuildingDescriber(GoodDescriber goodDescriber, ResourceAmountFormatter resourceAmountFormatter, ILoc loc, DescribedAmountFactory describedAmountFactory, ProductionItemFactory productionItemFactory)
	{
		_goodDescriber = goodDescriber;
		_resourceAmountFormatter = resourceAmountFormatter;
		_loc = loc;
		_describedAmountFactory = describedAmountFactory;
		_productionItemFactory = productionItemFactory;
	}

	public void Awake()
	{
		_goodConsumingBuilding = GetComponent<GoodConsumingBuilding>();
		_blockObject = GetComponent<BlockObject>();
		_workplace = GetComponent<Workplace>();
		_time = _loc.T(Phrase.New().FormatHours<int>(), 1);
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		if (_blockObject.IsPreview || _blockObject.IsUnfinished)
		{
			yield return DescribeSupply();
		}
		if (_blockObject.IsPreview && !_workplace)
		{
			string content = SpecialStrings.RowStarter + _loc.T(NeedsHaulersLocKey);
			yield return EntityDescription.CreateTextSection(content, 2030);
		}
	}

	private EntityDescription DescribeSupply()
	{
		return EntityDescription.CreateInputSectionWithTime(_productionItemFactory.CreateInput(CreateElements()), int.MaxValue, _time);
	}

	private IEnumerable<VisualElement> CreateElements()
	{
		foreach (ConsumedGoodSpec consumedGood in _goodConsumingBuilding.ConsumedGoods)
		{
			DescribedGood describedGood = _goodDescriber.GetDescribedGood(consumedGood.GoodId);
			float goodPerHour = consumedGood.GoodPerHour;
			string param = _resourceAmountFormatter.FormatPerHour(describedGood.DisplayName, goodPerHour);
			string tooltip = _loc.T(DescriptionLocKey, param);
			yield return _describedAmountFactory.CreatePlain(string.Empty, goodPerHour.ToString("0.##"), describedGood.Icon, tooltip);
		}
	}
}

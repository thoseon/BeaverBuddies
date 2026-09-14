using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.ScienceSystem;
using Timberborn.TooltipSystem;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.ScienceSystemUI;

public class ScienceNeedingBuildingDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string DescriptionLocKey = "GoodConsuming.SupplyDescription";

	private static readonly string ScienceClass = "described-amount--science";

	private static readonly string SciencePointsLocKey = "Science.SciencePoints";

	private readonly ProductionItemFactory _productionItemFactory;

	private readonly DescribedAmountFactory _describedAmountFactory;

	private readonly ResourceAmountFormatter _resourceAmountFormatter;

	private readonly ILoc _loc;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private ScienceNeedingBuilding _scienceNeedingBuilding;

	private BlockObject _blockObject;

	private string _time;

	public ScienceNeedingBuildingDescriber(ProductionItemFactory productionItemFactory, DescribedAmountFactory describedAmountFactory, ResourceAmountFormatter resourceAmountFormatter, ILoc loc, ITooltipRegistrar tooltipRegistrar)
	{
		_productionItemFactory = productionItemFactory;
		_describedAmountFactory = describedAmountFactory;
		_resourceAmountFormatter = resourceAmountFormatter;
		_loc = loc;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public void Awake()
	{
		_scienceNeedingBuilding = GetComponent<ScienceNeedingBuilding>();
		_blockObject = GetComponent<BlockObject>();
		_time = _loc.T(Phrase.New().FormatHours<int>(), 1);
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		if (_blockObject.IsPreview)
		{
			yield return CreateScienceUsageEntityDescription();
		}
	}

	public string DescribeScienceUsage()
	{
		int scienceUsedPerHour = _scienceNeedingBuilding.ScienceUsedPerHour;
		string param = _resourceAmountFormatter.FormatPerHour(_loc.T(SciencePointsLocKey), scienceUsedPerHour);
		return _loc.T(DescriptionLocKey, param);
	}

	private EntityDescription CreateScienceUsageEntityDescription()
	{
		int scienceUsedPerHour = _scienceNeedingBuilding.ScienceUsedPerHour;
		VisualElement visualElement = _describedAmountFactory.CreatePlain(ScienceClass, scienceUsedPerHour.ToString("0.#"));
		_tooltipRegistrar.Register(visualElement, DescribeScienceUsage());
		return EntityDescription.CreateInputSectionWithTime(_productionItemFactory.CreateInput(visualElement), 2147483646, _time);
	}
}

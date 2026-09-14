using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.PowerManagement;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.PowerManagementUI;

internal class GravityBatteryDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string CapacityClass = "described-amount--power";

	private static readonly string PowerCapacityLocKey = "Mechanical.PowerCapacity";

	private readonly ILoc _loc;

	private readonly DescribedAmountFactory _describedAmountFactory;

	private readonly ProductionItemFactory _productionItemFactory;

	private GravityBattery _gravityBattery;

	private readonly Phrase _capacityPhrase = Phrase.New().FormatPowerCapacityPerMeter<int>();

	public GravityBatteryDescriber(ILoc loc, DescribedAmountFactory describedAmountFactory, ProductionItemFactory productionItemFactory)
	{
		_loc = loc;
		_describedAmountFactory = describedAmountFactory;
		_productionItemFactory = productionItemFactory;
	}

	public void Awake()
	{
		_gravityBattery = GetComponent<GravityBattery>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		if ((bool)_gravityBattery)
		{
			string tooltipText = GetTooltipText();
			string amount = FormatCapacity(_gravityBattery.CapacityPerTile);
			VisualElement output = _describedAmountFactory.CreatePlain(CapacityClass, amount, tooltipText);
			VisualElement content = _productionItemFactory.CreateOutput(output);
			yield return EntityDescription.CreateOutputSection(content, 2147483646);
		}
	}

	private string GetTooltipText()
	{
		return _loc.T(PowerCapacityLocKey, FormatCapacity(_gravityBattery.CapacityPerTile));
	}

	private string FormatCapacity(int capacity)
	{
		return _loc.T(_capacityPhrase, capacity);
	}
}

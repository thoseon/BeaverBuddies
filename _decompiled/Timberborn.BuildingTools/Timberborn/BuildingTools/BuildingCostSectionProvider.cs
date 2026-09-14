using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.CoreUI;
using Timberborn.Goods;
using Timberborn.GoodsUI;
using UnityEngine.UIElements;

namespace Timberborn.BuildingTools;

public class BuildingCostSectionProvider
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly GoodItemFactory _goodItemFactory;

	public BuildingCostSectionProvider(VisualElementLoader visualElementLoader, GoodItemFactory goodItemFactory)
	{
		_visualElementLoader = visualElementLoader;
		_goodItemFactory = goodItemFactory;
	}

	public bool TryGetSection(Preview preview, out VisualElement section)
	{
		BuildingSpec component = preview.GetComponent<BuildingSpec>();
		string elementName = "Game/ToolPanel/DescriptionPanelCostSection";
		section = _visualElementLoader.LoadVisualElement(elementName);
		ImmutableArray<GoodAmountSpec> buildingCost = component.BuildingCost;
		bool num = buildingCost.Length > 0;
		if (num)
		{
			AddCost(buildingCost, section);
		}
		return num;
	}

	private void AddCost(IEnumerable<GoodAmountSpec> cost, VisualElement root)
	{
		VisualElement visualElement = root.Q<VisualElement>("Materials");
		foreach (GoodAmountSpec item in cost)
		{
			visualElement.Add(_goodItemFactory.Create(item, bordered: true));
		}
	}
}

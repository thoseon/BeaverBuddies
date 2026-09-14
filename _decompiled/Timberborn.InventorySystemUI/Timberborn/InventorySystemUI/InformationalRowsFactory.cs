using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Goods;
using Timberborn.GoodsUI;
using Timberborn.InventorySystem;
using UnityEngine.UIElements;

namespace Timberborn.InventorySystemUI;

public class InformationalRowsFactory
{
	private static readonly string InputClassName = "inventory-row-informational__type--input";

	private static readonly string OutputClassName = "inventory-row-informational__type--output";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly GoodDescriber _goodDescriber;

	public InformationalRowsFactory(VisualElementLoader visualElementLoader, GoodDescriber goodDescriber)
	{
		_visualElementLoader = visualElementLoader;
		_goodDescriber = goodDescriber;
	}

	public IEnumerable<InformationalRow> CreateRowsWithLimits(Inventory inventory, VisualElement parent)
	{
		return CreateRows(inventory, parent, withLimits: true);
	}

	public IEnumerable<InformationalRow> CreateRowsWithoutLimits(Inventory inventory, VisualElement parent)
	{
		return CreateRows(inventory, parent, withLimits: false);
	}

	public InformationalRow CreateInputRowWithLimit(StorableGood good, Inventory inventory, VisualElement parent)
	{
		return CreateInputRow(good, inventory, parent, withLimit: true);
	}

	public InformationalRow CreateOutputRowWithLimit(StorableGood good, Inventory inventory, VisualElement parent)
	{
		return CreateOutputRow(good, inventory, parent, withLimit: true);
	}

	public InformationalRow CreateSimpleRowWithoutLimit(StorableGood good, Inventory inventory, VisualElement parent)
	{
		return CreateInformationalRow(good, inventory, parent, withLimit: false);
	}

	private IEnumerable<InformationalRow> CreateRows(Inventory inventory, VisualElement parent, bool withLimits)
	{
		ReadOnlyList<StorableGoodAmount> goods = inventory.AllowedGoods;
		IEnumerable<StorableGood> enumerable = from good in goods
			where good.StorableGood.IsOnlyGivable
			select good.StorableGood;
		foreach (StorableGood item in enumerable)
		{
			yield return CreateInputRow(item, inventory, parent, withLimits);
		}
		IEnumerable<StorableGood> enumerable2 = from good in goods
			where good.StorableGood.IsOnlyTakeable
			select good.StorableGood;
		foreach (StorableGood item2 in enumerable2)
		{
			yield return CreateOutputRow(item2, inventory, parent, withLimits);
		}
	}

	private InformationalRow CreateInputRow(StorableGood good, Inventory inventory, VisualElement parent, bool withLimit)
	{
		InformationalRow informationalRow = CreateInformationalRow(good, inventory, parent, withLimit);
		informationalRow.Root.Q<Image>("Type").AddToClassList(InputClassName);
		return informationalRow;
	}

	private InformationalRow CreateOutputRow(StorableGood good, Inventory inventory, VisualElement parent, bool withLimit)
	{
		InformationalRow informationalRow = CreateInformationalRow(good, inventory, parent, withLimit);
		informationalRow.Root.Q<Image>("Type").AddToClassList(OutputClassName);
		return informationalRow;
	}

	private InformationalRow CreateInformationalRow(StorableGood good, Inventory inventory, VisualElement parent, bool withLimit)
	{
		string goodId = good.GoodId;
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/EntityPanel/InventoryInformationalRow");
		parent.Add(visualElement);
		DescribedGood describedGood = _goodDescriber.GetDescribedGood(goodId);
		visualElement.Q<Image>("Image").sprite = describedGood.Icon;
		visualElement.Q<Label>("Name").text = describedGood.DisplayName;
		return new InformationalRow(goodId, visualElement, visualElement.Q<Label>("Amount"), () => inventory.AmountInStock(goodId), withLimit, () => inventory.LimitedAmount(goodId), visualElement.Q<Label>("Limit"), visualElement.Q<Label>("Separator"));
	}
}

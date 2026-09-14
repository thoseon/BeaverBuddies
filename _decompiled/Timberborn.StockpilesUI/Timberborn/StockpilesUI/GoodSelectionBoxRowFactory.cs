using Timberborn.CoreUI;
using Timberborn.Goods;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

internal class GoodSelectionBoxRowFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly GoodsGroupSpecService _goodsGroupSpecService;

	public GoodSelectionBoxRowFactory(VisualElementLoader visualElementLoader, GoodsGroupSpecService goodsGroupSpecService)
	{
		_visualElementLoader = visualElementLoader;
		_goodsGroupSpecService = goodsGroupSpecService;
	}

	public GoodSelectionBoxRow Create(string goodGroupId)
	{
		GoodGroupSpec spec = _goodsGroupSpecService.GetSpec(goodGroupId);
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/EntityPanel/GoodSelectionBoxRow");
		visualElement.Q<Image>("HeaderIcon").sprite = spec.Icon.Asset;
		return new GoodSelectionBoxRow(visualElement, spec.Order, visualElement.Q<VisualElement>("Icons"));
	}
}

using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.StatusSystemUI;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

internal class StockpileGoodSelectionBoxFactory
{
	private readonly InputService _inputService;

	private readonly StatusListFragment _statusListFragment;

	private readonly StockpileGoodSelectionBoxItemsFactory _stockpileGoodSelectionBoxItemsFactory;

	private readonly VisualElementLoader _visualElementLoader;

	public StockpileGoodSelectionBoxFactory(InputService inputService, StatusListFragment statusListFragment, StockpileGoodSelectionBoxItemsFactory stockpileGoodSelectionBoxItemsFactory, VisualElementLoader visualElementLoader)
	{
		_inputService = inputService;
		_statusListFragment = statusListFragment;
		_stockpileGoodSelectionBoxItemsFactory = stockpileGoodSelectionBoxItemsFactory;
		_visualElementLoader = visualElementLoader;
	}

	public StockpileGoodSelectionBox Create()
	{
		string elementName = "Game/StockpileGoodSelectionBox";
		VisualElement root = _visualElementLoader.LoadVisualElement(elementName);
		StockpileGoodSelectionBox stockpileGoodSelectionBox = new StockpileGoodSelectionBox(_inputService, _statusListFragment, _stockpileGoodSelectionBoxItemsFactory, root);
		stockpileGoodSelectionBox.Initialize();
		return stockpileGoodSelectionBox;
	}
}

using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Stockpiles;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

public class StockpileInventoryFragment : IEntityPanelFragment
{
	private static readonly string ButtonHighlightClass = "highlight";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly IGoodSelectionController _goodSelectionController;

	private readonly IGoodService _goodService;

	private SingleGoodAllower _singleGoodAllower;

	private Inventory _inventory;

	private VisualElement _root;

	private Label _capacityAmount;

	private Label _capacityLimit;

	private Timberborn.CoreUI.ProgressBar _progressBar;

	private Button _goodSelectionButton;

	private Button _goodUnselectionButton;

	private VisualElement _outputGood;

	private Image _outputGoodIcon;

	private Label _outputGoodName;

	public StockpileInventoryFragment(VisualElementLoader visualElementLoader, IGoodSelectionController goodSelectionController, IGoodService goodService)
	{
		_visualElementLoader = visualElementLoader;
		_goodSelectionController = goodSelectionController;
		_goodService = goodService;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/StockpileInventoryFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_capacityAmount = _root.Q<Label>("CapacityAmount");
		_capacityLimit = _root.Q<Label>("CapacityLimit");
		_progressBar = _root.Q<Timberborn.CoreUI.ProgressBar>("ProgressBar");
		_goodSelectionButton = _root.Q<Button>("Selection");
		_goodUnselectionButton = _root.Q<Button>("Unselect");
		_goodUnselectionButton.RegisterCallback<ClickEvent>(delegate
		{
			_singleGoodAllower.Disallow();
		});
		_goodSelectionController.Initialize(_root);
		_outputGood = _root.Q<VisualElement>("OutputGood");
		_outputGoodIcon = _outputGood.Q<Image>("Image");
		_outputGoodName = _outputGood.Q<Label>("Name");
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		Stockpile component = entity.GetComponent<Stockpile>();
		if (component != null)
		{
			_singleGoodAllower = component.GetComponent<SingleGoodAllower>();
			_inventory = component.Inventory;
			_capacityLimit.text = _inventory.Capacity.ToString();
			_goodSelectionController.SetStockpile(component);
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void ClearFragment()
	{
		ToggleButtonHighlight(highlight: false);
		_singleGoodAllower = null;
		_inventory = null;
		_goodSelectionController.Clear();
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_inventory)
		{
			int totalAmountInStock = _inventory.TotalAmountInStock;
			_progressBar.SetProgress((float)totalAmountInStock / (float)_inventory.Capacity);
			_capacityAmount.text = totalAmountInStock.ToString();
			UpdateUnallowedGoods();
			_goodSelectionController.Update();
			_goodUnselectionButton.ToggleDisplayStyle(_singleGoodAllower.HasAllowedGood);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	public void ShowGoodSelectionBox()
	{
		_goodSelectionController.ShowGoodSelectionBox();
	}

	public void ToggleButtonHighlight(bool highlight)
	{
		if ((bool)_inventory)
		{
			_goodSelectionButton.EnableInClassList(ButtonHighlightClass, highlight);
		}
	}

	private void UpdateUnallowedGoods()
	{
		string allowedGood = _singleGoodAllower.AllowedGood;
		if (((allowedGood != null) ? _inventory.AmountInStock(allowedGood) : 0) == _inventory.TotalAmountInStock)
		{
			_outputGood.ToggleDisplayStyle(visible: false);
		}
		else
		{
			ShowOutputGood(allowedGood);
		}
	}

	private void ShowOutputGood(string allowedGood)
	{
		foreach (GoodAmount item in _inventory.Stock)
		{
			string goodId = item.GoodId;
			if (goodId != allowedGood)
			{
				GoodSpec good = _goodService.GetGood(goodId);
				_outputGood.ToggleDisplayStyle(visible: true);
				_outputGoodIcon.sprite = good.IconSmall.Value;
				_outputGoodName.text = good.PluralDisplayName.Value;
				break;
			}
		}
	}
}

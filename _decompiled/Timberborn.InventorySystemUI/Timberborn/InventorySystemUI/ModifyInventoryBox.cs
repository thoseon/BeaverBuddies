using System;
using System.Linq;
using System.Text;
using Timberborn.CoreUI;
using Timberborn.GameDistricts;
using Timberborn.Goods;
using Timberborn.GoodsUI;
using Timberborn.InputSystem;
using Timberborn.InventorySystem;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.InventorySystemUI;

internal class ModifyInventoryBox : IPanelController, ILoadableSingleton
{
	private static readonly string InventoryGoodAmountMultiplierKey = "InventoryGoodAmountMultiplier";

	private static readonly string InventorySubtractGoodKey = "InventorySubtractGood";

	private static readonly string DefaultAmount = "10";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly PanelStack _panelStack;

	private readonly InputService _inputService;

	private readonly GoodDescriber _goodDescriber;

	private Inventory _inventory;

	private SingleGoodAllower _singleGoodAllower;

	private VisualElement _root;

	private TextField _amount;

	private VisualElement _buttons;

	private Label _inventoryContents;

	private Label _warning;

	private int MaxAmountPerGood => int.MaxValue / _inventory.AllowedGoods.Count();

	public ModifyInventoryBox(VisualElementLoader visualElementLoader, PanelStack panelStack, InputService inputService, GoodDescriber goodDescriber)
	{
		_visualElementLoader = visualElementLoader;
		_panelStack = panelStack;
		_inputService = inputService;
		_goodDescriber = goodDescriber;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/ModifyInventory/ModifyInventoryBox");
		_root.Q<Button>("CancelButton").RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		_amount = _root.Q<TextField>("Amount");
		_amount.SetValueWithoutNotify(DefaultAmount);
		_buttons = _root.Q<VisualElement>("Buttons");
		_inventoryContents = _root.Q<Label>("Inventory");
		_warning = _root.Q<Label>("Warning");
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public void Open(Inventory inventory)
	{
		_inventory = inventory;
		_singleGoodAllower = _inventory.GetComponent<SingleGoodAllower>();
		CreateButtons();
		UpdateContents();
		_panelStack.PushOverlay(this);
	}

	public bool OnUIConfirmed()
	{
		return false;
	}

	public void OnUICancelled()
	{
		_buttons.Clear();
		_inventory = null;
		_singleGoodAllower = null;
		_panelStack.Pop(this);
	}

	private void CreateButtons()
	{
		CreateCustomButtons();
		foreach (StorableGoodAmount allowedGood in _inventory.AllowedGoods)
		{
			if (!_singleGoodAllower || !_singleGoodAllower.HasAllowedGood || _singleGoodAllower.AllowedGood == allowedGood.StorableGood.GoodId)
			{
				CreateGoodButton(allowedGood.StorableGood.GoodId);
			}
		}
	}

	private void CreateCustomButtons()
	{
		CreateCustomButton("<b>GIVE ALL</b>", GiveAll);
		CreateCustomButton("<b>GIVE INPUT</b>", GiveInput);
		CreateCustomButton("<b>CLEAR ALL</b>", ClearAll);
	}

	private void CreateCustomButton(string text, EventCallback<ClickEvent> callback)
	{
		Button button = _visualElementLoader.LoadVisualElement("Game/ModifyInventory/ModifyInventoryBoxButton").Q<Button>("Button");
		button.text = text;
		button.RegisterCallback(callback);
		_buttons.Add(button);
	}

	private void CreateGoodButton(string goodId)
	{
		Button button = _visualElementLoader.LoadVisualElement("Game/ModifyInventory/ModifyInventoryBoxButton").Q<Button>("Button");
		button.text = _goodDescriber.Describe(goodId);
		button.RegisterCallback<ClickEvent>(delegate
		{
			OnGoodButtonClick(goodId);
		});
		_buttons.Add(button);
	}

	private void OnGoodButtonClick(string goodId)
	{
		UpdateAllowedGood(goodId);
		int num = ((!_inputService.IsKeyHeld(InventoryGoodAmountMultiplierKey)) ? 1 : 10);
		int amount = int.Parse(_amount.text) * num;
		GoodAmount goodAmount = new GoodAmount(goodId, amount);
		if (goodAmount.Amount > 0)
		{
			_warning.text = "";
			if (_inputService.IsKeyHeld(InventorySubtractGoodKey))
			{
				TakeGood(goodAmount);
			}
			else
			{
				GiveGood(goodAmount);
			}
		}
		else
		{
			_warning.text = "The amount must be positive.";
		}
	}

	private void GiveAll(ClickEvent evt)
	{
		foreach (StorableGoodAmount allowedGood in _inventory.AllowedGoods)
		{
			GiveGood(new GoodAmount(allowedGood.StorableGood.GoodId, MaxAmountPerGood));
		}
	}

	private void GiveInput(ClickEvent evt)
	{
		foreach (string inputGood in _inventory.InputGoods)
		{
			GiveGood(new GoodAmount(inputGood, MaxAmountPerGood));
		}
	}

	private void ClearAll(ClickEvent evt)
	{
		foreach (StorableGoodAmount allowedGood in _inventory.AllowedGoods)
		{
			TakeGood(new GoodAmount(allowedGood.StorableGood.GoodId, MaxAmountPerGood));
		}
	}

	private void UpdateAllowedGood(string goodId)
	{
		if ((bool)_singleGoodAllower && !_singleGoodAllower.HasAllowedGood)
		{
			_singleGoodAllower.Allow(goodId);
			_buttons.Clear();
			CreateButtons();
		}
	}

	private void GiveGood(GoodAmount goodAmount)
	{
		int num = _inventory.UnreservedCapacity(goodAmount.GoodId);
		if (_inventory.HasComponent<DistrictCenter>())
		{
			int num2 = MaxAmountPerGood - _inventory.AmountInStock(goodAmount.GoodId);
			if (num2 > 0)
			{
				_inventory.GiveExistingIgnoringCapacity(new GoodAmount(goodAmount.GoodId, Math.Min(num2, goodAmount.Amount)));
				UpdateContents();
			}
		}
		else if (num > 0)
		{
			_inventory.GiveExisting(new GoodAmount(goodAmount.GoodId, Math.Min(num, goodAmount.Amount)));
			UpdateContents();
		}
	}

	private void TakeGood(GoodAmount goodAmount)
	{
		int num = _inventory.UnreservedAmountInStock(goodAmount.GoodId);
		if (num > 0)
		{
			_inventory.TakeConsumed(new GoodAmount(goodAmount.GoodId, Math.Min(num, goodAmount.Amount)));
			UpdateContents();
		}
	}

	private void UpdateContents()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"Stock ({_inventory.TotalAmountInStock} / {_inventory.Capacity}):");
		foreach (GoodAmount item in _inventory.Stock)
		{
			stringBuilder.AppendLine("  " + _goodDescriber.Describe(item));
		}
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("Reserved capacity:");
		foreach (GoodAmount item2 in _inventory.ReservedCapacity())
		{
			stringBuilder.AppendLine("  " + _goodDescriber.Describe(item2));
		}
		_inventoryContents.text = stringBuilder.ToString();
	}
}

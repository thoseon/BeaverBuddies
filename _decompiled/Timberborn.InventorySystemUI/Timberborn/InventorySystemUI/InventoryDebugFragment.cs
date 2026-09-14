using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Goods;
using Timberborn.GoodsUI;
using Timberborn.InventorySystem;
using UnityEngine.UIElements;

namespace Timberborn.InventorySystemUI;

internal class InventoryDebugFragment : IEntityPanelFragment
{
	private readonly DebugFragmentFactory _debugFragmentFactory;

	private readonly IGoodService _goodService;

	private readonly GoodDescriber _goodDescriber;

	private readonly ModifyInventoryBox _modifyInventoryBox;

	private Label _text;

	private readonly List<Inventory> _inventories = new List<Inventory>();

	private readonly StringBuilder _description = new StringBuilder();

	private VisualElement _root;

	public InventoryDebugFragment(DebugFragmentFactory debugFragmentFactory, IGoodService goodService, GoodDescriber goodDescriber, ModifyInventoryBox modifyInventoryBox)
	{
		_debugFragmentFactory = debugFragmentFactory;
		_goodService = goodService;
		_goodDescriber = goodDescriber;
		_modifyInventoryBox = modifyInventoryBox;
	}

	public VisualElement InitializeFragment()
	{
		DebugFragmentButton debugFragmentButton = new DebugFragmentButton(OnModifyInventoryButtonClick, "Modify Inventory");
		_root = _debugFragmentFactory.Create("Inventory", debugFragmentButton);
		_text = _root.Q<Label>("Text");
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		entity.GetComponents(_inventories);
	}

	public void ClearFragment()
	{
		_inventories.Clear();
		UpdateContent();
	}

	public void UpdateFragment()
	{
		UpdateContent();
	}

	private void OnModifyInventoryButtonClick()
	{
		Inventory inventory = _inventories.SingleOrDefault((Inventory inventory2) => (bool)inventory2 && inventory2.Enabled);
		if ((bool)inventory)
		{
			_modifyInventoryBox.Open(inventory);
		}
	}

	private void UpdateContent()
	{
		_description.Clear();
		if (_inventories != null)
		{
			foreach (Inventory inventory in _inventories)
			{
				if ((bool)inventory)
				{
					DescribeInventory(inventory, _description);
				}
			}
		}
		if (_description.Length > 0)
		{
			_text.text = _description.ToStringWithoutNewLineEnd();
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void DescribeInventory(Inventory inventory, StringBuilder description)
	{
		description.Append(inventory.ComponentName);
		description.AppendLine(inventory.Enabled ? " (on)" : " (off)");
		foreach (string good in _goodService.Goods)
		{
			DescribeGood(inventory, good, description);
		}
	}

	private void DescribeGood(Inventory inventory, string goodId, StringBuilder description)
	{
		int num = inventory.AmountInStock(goodId);
		int num2 = inventory.ReservedCapacity(goodId);
		if (num > 0 || num2 > 0)
		{
			int num3 = num - inventory.UnreservedAmountInStock(goodId);
			string value = $"{_goodDescriber.Describe(goodId)}: {num}" + $" ({num3} reserved, {num2} incoming)";
			description.AppendLine(value);
		}
	}
}

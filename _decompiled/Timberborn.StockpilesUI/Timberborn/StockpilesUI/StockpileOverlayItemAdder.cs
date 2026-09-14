using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.SelectionSystem;
using Timberborn.Stockpiles;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

internal class StockpileOverlayItemAdder : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity
{
	private static readonly string IconHiddenClass = "icon--hidden";

	private readonly StockpileOverlay _stockpileOverlay;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly IGoodService _goodService;

	private readonly EntitySelectionService _entitySelectionService;

	private BlockObjectCenter _blockObjectCenter;

	private SingleGoodAllower _singleGoodAllower;

	private Stockpile _stockpile;

	private BlockObjectModelController _blockObjectModelController;

	private BlockObject _blockObject;

	private VisualElement _item;

	private Image _itemIcon;

	private Label _itemText;

	private VisualElement _fillLevel;

	private Button _selectionButton;

	private Inventory Inventory => _stockpile.Inventory;

	public StockpileOverlayItemAdder(StockpileOverlay stockpileOverlay, VisualElementLoader visualElementLoader, IGoodService goodService, EntitySelectionService entitySelectionService)
	{
		_stockpileOverlay = stockpileOverlay;
		_visualElementLoader = visualElementLoader;
		_goodService = goodService;
		_entitySelectionService = entitySelectionService;
	}

	public void Awake()
	{
		_blockObjectCenter = GetComponent<BlockObjectCenter>();
		_singleGoodAllower = GetComponent<SingleGoodAllower>();
		_stockpile = GetComponent<Stockpile>();
		_blockObjectModelController = GetComponent<BlockObjectModelController>();
		_blockObject = GetComponent<BlockObject>();
		VisualElement e = _visualElementLoader.LoadVisualElement("Game/StockpileOverlayItem");
		_item = e.Q<VisualElement>("StockpileOverlayItem");
		_item.Q<Button>("EntityButton").RegisterCallback<ClickEvent>(delegate
		{
			_entitySelectionService.Select(_stockpile);
		});
		_selectionButton = _item.Q<Button>("SelectionButton");
		_selectionButton.RegisterCallback<ClickEvent>(delegate
		{
			_stockpileOverlay.ToggleGoodSelection(_stockpile, _item);
		});
		_itemIcon = _item.Q<Image>("Icon");
		_itemText = _item.Q<Label>("Stock");
		_fillLevel = _item.Q<VisualElement>("Progress");
	}

	public void InitializeEntity()
	{
		_singleGoodAllower.DisallowedGoodsChanged += OnDisallowedGoodsChanged;
		Inventory.InventoryChanged += OnInventoryChanged;
		_blockObjectModelController.ModelsUpdated += OnModelsUpdated;
		Add();
		UpdateIcon();
		UpdateAmount();
	}

	public void DeleteEntity()
	{
		_singleGoodAllower.DisallowedGoodsChanged -= OnDisallowedGoodsChanged;
		Inventory.InventoryChanged -= OnInventoryChanged;
		_blockObjectModelController.ModelsUpdated -= OnModelsUpdated;
		Remove();
	}

	private void OnDisallowedGoodsChanged(object sender, DisallowedGoodsChangedEventArgs e)
	{
		UpdateIcon();
	}

	private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
	{
		UpdateAmount();
	}

	private void UpdateIcon()
	{
		if (_singleGoodAllower.HasAllowedGood)
		{
			GoodSpec good = _goodService.GetGood(_singleGoodAllower.AllowedGood);
			_itemIcon.sprite = good.IconSmall.Value;
			_itemIcon.AddToClassList(IconHiddenClass);
		}
		else
		{
			_itemIcon.RemoveFromClassList(IconHiddenClass);
			_itemIcon.sprite = null;
		}
	}

	private void UpdateAmount()
	{
		if (_singleGoodAllower.HasAllowedGood && _blockObject.IsFinished)
		{
			int num = Inventory.AmountInStock(_singleGoodAllower.AllowedGood);
			_itemText.text = num.ToString();
			_fillLevel.SetHeightAsPercent((float)num / (float)Inventory.Capacity);
			_fillLevel.parent.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_itemText.text = "0";
			_fillLevel.parent.ToggleDisplayStyle(visible: false);
		}
	}

	private void OnModelsUpdated(object sender, EventArgs e)
	{
		if (_blockObjectModelController.IsAnyModelShown)
		{
			Add();
		}
		else
		{
			Remove();
		}
		UpdateIcon();
		UpdateAmount();
	}

	private void Add()
	{
		Vector3 worldCenter = _blockObjectCenter.WorldCenter;
		Vector3 worldCenterGrounded = _blockObjectCenter.WorldCenterGrounded;
		float y = (worldCenter.y + worldCenterGrounded.y) * 0.5f;
		Vector3 anchor = new Vector3(worldCenter.x, y, worldCenter.z);
		_stockpileOverlay.Add(_item, anchor);
	}

	private void Remove()
	{
		_stockpileOverlay.Remove(_item);
	}
}

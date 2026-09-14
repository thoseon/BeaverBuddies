using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.InventorySystem;
using Timberborn.StatusSystemUI;
using Timberborn.Stockpiles;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

internal class StockpileGoodSelectionBox : IInputProcessor
{
	private static readonly string NoMarginClass = "good-selection-box-row--no-margin";

	private readonly InputService _inputService;

	private readonly StatusListFragment _statusListFragment;

	private readonly StockpileGoodSelectionBoxItemsFactory _stockpileGoodSelectionBoxItemsFactory;

	private VisualElement _goodSelection;

	private readonly List<GoodSelectionBoxRow> _rows = new List<GoodSelectionBoxRow>();

	private SingleGoodAllower _singleGoodAllower;

	private bool _isMouseOverElement;

	private bool _ignoreInput;

	public VisualElement Root { get; }

	private bool IsShown => Root.IsDisplayed();

	private bool ShouldProcessInput
	{
		get
		{
			if (!_isMouseOverElement)
			{
				return !_ignoreInput;
			}
			return false;
		}
	}

	public event EventHandler SelectionBoxClosed;

	public StockpileGoodSelectionBox(InputService inputService, StatusListFragment statusListFragment, StockpileGoodSelectionBoxItemsFactory stockpileGoodSelectionBoxItemsFactory, VisualElement root)
	{
		_inputService = inputService;
		_statusListFragment = statusListFragment;
		_stockpileGoodSelectionBoxItemsFactory = stockpileGoodSelectionBoxItemsFactory;
		Root = root;
	}

	public void Initialize()
	{
		_goodSelection = Root.Q<VisualElement>("GoodSelection");
		Root.ToggleDisplayStyle(visible: false);
		Root.RegisterCallback<MouseEnterEvent>(delegate
		{
			_isMouseOverElement = true;
		});
		Root.RegisterCallback<MouseLeaveEvent>(delegate
		{
			_isMouseOverElement = false;
		});
	}

	public bool ProcessInput()
	{
		if (_inputService.UICancel || (_inputService.MainMouseButtonDown && ShouldProcessInput))
		{
			HideGoodSelection();
			return _inputService.UICancel;
		}
		return false;
	}

	public void Update()
	{
		if (IsShown)
		{
			UpdateRows();
		}
	}

	public void ToggleGoodSelection(Stockpile stockpile)
	{
		if (IsShown)
		{
			HideGoodSelection();
		}
		else
		{
			ShowGoodSelection(stockpile);
		}
	}

	public void Hide()
	{
		if (IsShown)
		{
			HideGoodSelection();
		}
	}

	public void DisableInput()
	{
		_ignoreInput = true;
	}

	public void EnableInput()
	{
		_ignoreInput = false;
	}

	private void HideGoodSelection()
	{
		Root.ToggleDisplayStyle(visible: false);
		_inputService.RemoveInputProcessor(this);
		_goodSelection.Clear();
		_rows.Clear();
		if ((bool)_singleGoodAllower)
		{
			_singleGoodAllower.DisallowedGoodsChanged -= OnDisallowedGoodsChanged;
		}
		_singleGoodAllower = null;
		SelectionBoxClosed?.Invoke(this, EventArgs.Empty);
	}

	private void ShowGoodSelection(Stockpile stockpile)
	{
		_ignoreInput = true;
		_isMouseOverElement = false;
		_singleGoodAllower = stockpile.GetComponent<SingleGoodAllower>();
		_singleGoodAllower.DisallowedGoodsChanged += OnDisallowedGoodsChanged;
		AddItems(stockpile);
		Root.ToggleDisplayStyle(visible: true);
		_inputService.AddInputProcessor(this);
	}

	private void OnDisallowedGoodsChanged(object sender, DisallowedGoodsChangedEventArgs e)
	{
		UpdateSelection();
	}

	private void AddItems(Stockpile stockpile)
	{
		IEnumerable<GoodSelectionBoxRow> collection = _stockpileGoodSelectionBoxItemsFactory.CreateItems(stockpile, SetGood, _goodSelection);
		_rows.AddRange(collection);
		_rows.Last().Root.AddToClassList(NoMarginClass);
		UpdateSelection();
		UpdateRows();
	}

	private void SetGood(string value)
	{
		_singleGoodAllower.Allow(value);
		HideGoodSelection();
		_statusListFragment.UpdateFragment();
	}

	private void UpdateSelection()
	{
		string selectedGoodId = _singleGoodAllower.AllowedGood ?? StockpileOptionsService.NothingSelectedLocKey;
		foreach (GoodSelectionBoxRow row in _rows)
		{
			row.UpdateSelectedState(selectedGoodId);
		}
	}

	private void UpdateRows()
	{
		for (int i = 0; i < _rows.Count; i++)
		{
			_rows[i].Update();
		}
	}
}

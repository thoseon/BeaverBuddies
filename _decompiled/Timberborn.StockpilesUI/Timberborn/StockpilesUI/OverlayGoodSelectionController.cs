using System;
using Timberborn.SingletonSystem;
using Timberborn.Stockpiles;
using Timberborn.TickSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

internal class OverlayGoodSelectionController : ILoadableSingleton, ILateUpdatableSingleton, ITickableSingleton
{
	private static readonly float BottomVerticalOffset = 5f;

	private static readonly float HorizontalBoxSpacing = 3f;

	private readonly UILayout _uiLayout;

	private readonly StockpileGoodSelectionBoxFactory _stockpileGoodSelectionBoxFactory;

	private StockpileGoodSelectionBox _stockpileGoodSelectionBox;

	private bool _isDirty;

	public VisualElement SelectedItem { get; private set; }

	private IResolvedStyle BoxResolvedStyle => _stockpileGoodSelectionBox.Root.resolvedStyle;

	public OverlayGoodSelectionController(UILayout uiLayout, StockpileGoodSelectionBoxFactory stockpileGoodSelectionBoxFactory)
	{
		_uiLayout = uiLayout;
		_stockpileGoodSelectionBoxFactory = stockpileGoodSelectionBoxFactory;
	}

	public void Load()
	{
		_stockpileGoodSelectionBox = _stockpileGoodSelectionBoxFactory.Create();
		_stockpileGoodSelectionBox.SelectionBoxClosed += OnSelectionBoxClosed;
		_uiLayout.AddAbsoluteItem(_stockpileGoodSelectionBox.Root);
	}

	public void Tick()
	{
		_stockpileGoodSelectionBox.Update();
	}

	public void LateUpdateSingleton()
	{
		if (_isDirty)
		{
			UpdatePosition();
		}
	}

	public void ToggleGoodSelection(Stockpile stockpile, VisualElement item)
	{
		SetSelectedItem(item);
		_stockpileGoodSelectionBox.ToggleGoodSelection(stockpile);
		_stockpileGoodSelectionBox.Root.visible = false;
		_isDirty = true;
	}

	public void HideBox()
	{
		ClearSelectedItem();
		_stockpileGoodSelectionBox.Hide();
	}

	private void OnSelectionBoxClosed(object sender, EventArgs e)
	{
		ClearSelectedItem();
	}

	private void SetSelectedItem(VisualElement selectedItem)
	{
		ClearSelectedItem();
		SelectedItem = selectedItem;
		SelectedItem.RegisterCallback<MouseEnterEvent>(OnMouseEnterSelectedItem);
		SelectedItem.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveSelectedItem);
	}

	private void ClearSelectedItem()
	{
		if (SelectedItem != null)
		{
			SelectedItem.UnregisterCallback<MouseEnterEvent>(OnMouseEnterSelectedItem);
			SelectedItem.UnregisterCallback<MouseLeaveEvent>(OnMouseLeaveSelectedItem);
			SelectedItem = null;
		}
	}

	private void OnMouseEnterSelectedItem(MouseEnterEvent evt)
	{
		_stockpileGoodSelectionBox.DisableInput();
	}

	private void OnMouseLeaveSelectedItem(MouseLeaveEvent evt)
	{
		_stockpileGoodSelectionBox.EnableInput();
	}

	private void UpdatePosition()
	{
		if (SelectedItem != null)
		{
			_stockpileGoodSelectionBox.Root.style.left = CalculateHorizontalPosition();
			_stockpileGoodSelectionBox.Root.style.top = CalculateVerticalPosition();
			_stockpileGoodSelectionBox.Root.visible = true;
		}
		_isDirty = false;
	}

	private float CalculateHorizontalPosition()
	{
		float width = BoxResolvedStyle.width;
		float x = SelectedItem.worldBound.x;
		float num = x - width - HorizontalBoxSpacing;
		if (num > 0f)
		{
			return num;
		}
		return x + SelectedItem.resolvedStyle.width + HorizontalBoxSpacing;
	}

	private float CalculateVerticalPosition()
	{
		float height = _stockpileGoodSelectionBox.Root.parent.resolvedStyle.height;
		float height2 = BoxResolvedStyle.height;
		return Math.Min(SelectedItem.worldBound.y, height - height2 - BottomVerticalOffset);
	}
}

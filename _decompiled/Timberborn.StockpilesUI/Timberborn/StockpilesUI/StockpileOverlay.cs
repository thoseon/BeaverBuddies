using System.Collections.Generic;
using Timberborn.CameraSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.Stockpiles;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

internal class StockpileOverlay : ILoadableSingleton, ILateUpdatableSingleton
{
	private readonly CameraService _cameraService;

	private readonly OverlayGoodSelectionController _overlayGoodSelectionController;

	private readonly UISettings _uiSettings;

	private readonly Underlay _underlay;

	private readonly Dictionary<VisualElement, Vector3> _items = new Dictionary<VisualElement, Vector3>();

	private readonly List<StockpileOverlayToggle> _toggles = new List<StockpileOverlayToggle>();

	private bool _enabled;

	private bool _isDirty;

	public StockpileOverlay(CameraService cameraService, OverlayGoodSelectionController overlayGoodSelectionController, UISettings uiSettings, Underlay underlay)
	{
		_cameraService = cameraService;
		_overlayGoodSelectionController = overlayGoodSelectionController;
		_uiSettings = uiSettings;
		_underlay = underlay;
	}

	public void Load()
	{
		_cameraService.CameraPositionOrRotationChanged += delegate
		{
			UpdatePosition();
		};
		_uiSettings.UIScaleFactorChanged += delegate
		{
			UpdatePosition();
		};
	}

	public void LateUpdateSingleton()
	{
		if (_isDirty)
		{
			UpdatePosition();
		}
	}

	public StockpileOverlayToggle GetStockpileOverlayToggle()
	{
		StockpileOverlayToggle stockpileOverlayToggle = new StockpileOverlayToggle();
		_toggles.Add(stockpileOverlayToggle);
		stockpileOverlayToggle.StateChanged += delegate
		{
			UpdateOverlay();
		};
		return stockpileOverlayToggle;
	}

	public void Add(VisualElement element, Vector3 anchor)
	{
		if (_items.TryAdd(element, anchor) && _enabled)
		{
			_underlay.Add(element);
			_isDirty = true;
		}
	}

	public void Remove(VisualElement element)
	{
		if (_items.Remove(element) && _enabled)
		{
			_underlay.Remove(element);
			if (element == _overlayGoodSelectionController.SelectedItem)
			{
				_overlayGoodSelectionController.HideBox();
			}
		}
	}

	public void ToggleGoodSelection(Stockpile stockpile, VisualElement item)
	{
		if (_enabled)
		{
			_overlayGoodSelectionController.ToggleGoodSelection(stockpile, item);
		}
	}

	private void UpdatePosition()
	{
		if (_enabled)
		{
			foreach (var (item, anchor) in _items)
			{
				UpdatePosition(item, anchor);
			}
			_overlayGoodSelectionController.HideBox();
		}
		_isDirty = false;
	}

	private void UpdateOverlay()
	{
		bool flag = _toggles.FastAny((StockpileOverlayToggle toggle) => toggle.Enabled) && _toggles.FastAll((StockpileOverlayToggle toggle) => !toggle.Hidden);
		if (flag && !_enabled)
		{
			Enable();
		}
		else if (!flag && _enabled)
		{
			Disable();
		}
	}

	private void Enable()
	{
		_enabled = true;
		foreach (VisualElement key in _items.Keys)
		{
			_underlay.Add(key);
		}
		_isDirty = true;
	}

	private void Disable()
	{
		_enabled = false;
		foreach (VisualElement key in _items.Keys)
		{
			_underlay.Remove(key);
		}
		_overlayGoodSelectionController.HideBox();
	}

	private void UpdatePosition(VisualElement item, Vector3 anchor)
	{
		VisualElement root = _underlay.Root;
		if (item.panel != null)
		{
			bool flag = _cameraService.IsInFront(anchor);
			item.ToggleDisplayStyle(flag);
			if (flag)
			{
				Vector3 vector = _cameraService.WorldSpaceToPanelSpace(root, anchor);
				item.style.translate = new Vector2(vector.x - root.layout.width / 2f, vector.y - root.layout.height / 2f);
			}
		}
	}
}

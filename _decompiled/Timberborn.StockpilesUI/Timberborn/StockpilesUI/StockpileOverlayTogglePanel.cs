using System;
using Timberborn.CoreUI;
using Timberborn.InputSystemUI;
using Timberborn.KeyBindingSystemUI;
using Timberborn.SingletonSystem;
using Timberborn.TooltipSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

internal class StockpileOverlayTogglePanel : ILoadableSingleton
{
	private static readonly string StockpileOverlayClass = "square-toggle--stockpile-overlay";

	private static readonly string ShowOverlayLocKey = "Inventory.StockpileOverlay.Show";

	private static readonly string HideOverlayLocKey = "Inventory.StockpileOverlay.Hide";

	private static readonly string ToggleStockpileOverlayKey = "ToggleStockpileOverlay";

	private static readonly string ShowStockpileOverlayKey = "ShowStockpileOverlay";

	private readonly StockpileOverlay _stockpileOverlay;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly UILayout _uiLayout;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly BindableToggleFactory _bindableToggleFactory;

	private readonly KeyBindingTooltipFactory _keyBindingTooltipFactory;

	private readonly EventBus _eventBus;

	private StockpileOverlayToggle _stockpileOverlayToggle;

	private bool _enabled;

	private VisualElement _root;

	private string TooltipLocKey
	{
		get
		{
			if (!_enabled)
			{
				return ShowOverlayLocKey;
			}
			return HideOverlayLocKey;
		}
	}

	public StockpileOverlayTogglePanel(StockpileOverlay stockpileOverlay, VisualElementLoader visualElementLoader, UILayout uiLayout, ITooltipRegistrar tooltipRegistrar, BindableToggleFactory bindableToggleFactory, KeyBindingTooltipFactory keyBindingTooltipFactory, EventBus eventBus)
	{
		_stockpileOverlay = stockpileOverlay;
		_visualElementLoader = visualElementLoader;
		_uiLayout = uiLayout;
		_tooltipRegistrar = tooltipRegistrar;
		_bindableToggleFactory = bindableToggleFactory;
		_keyBindingTooltipFactory = keyBindingTooltipFactory;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/SquareToggle");
		_tooltipRegistrar.Register(_root, (Func<string>)GetTooltip);
		Toggle toggle = _root.Q<Toggle>("Toggle");
		toggle.AddToClassList(StockpileOverlayClass);
		_bindableToggleFactory.CreateAndBind(toggle, ToggleStockpileOverlayKey, OnOverlayToggled, () => _enabled);
		_stockpileOverlayToggle = _stockpileOverlay.GetStockpileOverlayToggle();
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_uiLayout.AddTopRightButton(_root, 2);
	}

	private void OnOverlayToggled(bool showOverlay)
	{
		if (showOverlay)
		{
			_stockpileOverlayToggle.EnableOverlay();
			_enabled = true;
		}
		else
		{
			_stockpileOverlayToggle.DisableOverlay();
			_enabled = false;
		}
	}

	private string GetTooltip()
	{
		return _keyBindingTooltipFactory.Create(TooltipLocKey, ToggleStockpileOverlayKey, ShowStockpileOverlayKey);
	}
}

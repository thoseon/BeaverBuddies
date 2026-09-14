using System;
using Timberborn.CoreUI;
using Timberborn.InputSystemUI;
using Timberborn.KeyBindingSystemUI;
using Timberborn.SingletonSystem;
using Timberborn.TooltipSystem;
using Timberborn.UILayoutSystem;
using Timberborn.WaterSystemRendering;
using UnityEngine.UIElements;

namespace Timberborn.WaterSystemUI;

internal class WaterOpacityTogglePanel : ILoadableSingleton
{
	private static readonly string WaterOpacityClass = "square-toggle--water-opacity";

	private static readonly string ShowWaterLocKey = "WaterOpacity.Visibility.Show";

	private static readonly string HideWaterLocKey = "WaterOpacity.Visibility.Hide";

	private static readonly string ToggleWaterVisibilityKey = "ToggleWaterVisibility";

	private readonly WaterOpacityService _waterOpacityService;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly UILayout _uiLayout;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly BindableToggleFactory _bindableToggleFactory;

	private readonly KeyBindingTooltipFactory _keyBindingTooltipFactory;

	private readonly EventBus _eventBus;

	private WaterOpacityToggle _waterOpacityToggle;

	private VisualElement _root;

	private Toggle _toggle;

	private string TooltipLocKey
	{
		get
		{
			if (!_waterOpacityToggle.Hidden)
			{
				return HideWaterLocKey;
			}
			return ShowWaterLocKey;
		}
	}

	public WaterOpacityTogglePanel(WaterOpacityService waterOpacityService, VisualElementLoader visualElementLoader, UILayout uiLayout, ITooltipRegistrar tooltipRegistrar, BindableToggleFactory bindableToggleFactory, KeyBindingTooltipFactory keyBindingTooltipFactory, EventBus eventBus)
	{
		_waterOpacityService = waterOpacityService;
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
		_toggle = _root.Q<Toggle>("Toggle");
		_toggle.AddToClassList(WaterOpacityClass);
		_waterOpacityToggle = _waterOpacityService.GetWaterOpacityToggle();
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_bindableToggleFactory.CreateAndBind(_toggle, ToggleWaterVisibilityKey, OnWaterToggled, () => _waterOpacityToggle.Hidden);
		_uiLayout.AddTopRightButton(_root, 1);
	}

	private void OnWaterToggled(bool hideWater)
	{
		if (hideWater)
		{
			_waterOpacityToggle.HideWater();
		}
		else
		{
			_waterOpacityToggle.ShowWater();
		}
	}

	private string GetTooltip()
	{
		return _keyBindingTooltipFactory.Create(TooltipLocKey, ToggleWaterVisibilityKey, null);
	}
}

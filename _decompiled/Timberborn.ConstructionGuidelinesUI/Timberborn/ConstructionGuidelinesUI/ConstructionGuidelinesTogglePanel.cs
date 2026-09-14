using System;
using Timberborn.ConstructionGuidelines;
using Timberborn.CoreUI;
using Timberborn.InputSystemUI;
using Timberborn.KeyBindingSystemUI;
using Timberborn.SingletonSystem;
using Timberborn.TooltipSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.ConstructionGuidelinesUI;

internal class ConstructionGuidelinesTogglePanel : ILoadableSingleton
{
	private static readonly string ConstructionGuidelinesClass = "square-toggle--construction-guidelines";

	private static readonly string ShowGridRenderingLocKey = "GridRendering.Visibility.Show";

	private static readonly string HideGridRenderingLocKey = "GridRendering.Visibility.Hide";

	private static readonly string ToggleGuidelinesKey = "ToggleGuidelines";

	private static readonly string ShowGuidelinesKey = "ShowGuidelines";

	private readonly ConstructionGuidelinesRenderingService _constructionGuidelinesRenderingService;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly UILayout _uiLayout;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly BindableToggleFactory _bindableToggleFactory;

	private readonly KeyBindingTooltipFactory _keyBindingTooltipFactory;

	private readonly EventBus _eventBus;

	private VisualElement _root;

	private bool _inConstructionMode;

	private Toggle _toggle;

	private bool _enabled;

	public ConstructionGuidelinesTogglePanel(ConstructionGuidelinesRenderingService constructionGuidelinesRenderingService, VisualElementLoader visualElementLoader, UILayout uiLayout, ITooltipRegistrar tooltipRegistrar, BindableToggleFactory bindableToggleFactory, KeyBindingTooltipFactory keyBindingTooltipFactory, EventBus eventBus)
	{
		_constructionGuidelinesRenderingService = constructionGuidelinesRenderingService;
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
		_toggle.AddToClassList(ConstructionGuidelinesClass);
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_bindableToggleFactory.CreateAndBind(_toggle, ToggleGuidelinesKey, OnGridToggled, () => _enabled);
		_uiLayout.AddTopRightButton(_root, 3);
	}

	private void OnGridToggled(bool toggleState)
	{
		if (toggleState)
		{
			_constructionGuidelinesRenderingService.EnableGuidelines();
			_enabled = true;
		}
		else
		{
			_constructionGuidelinesRenderingService.DisableGuidelines();
			_enabled = false;
		}
	}

	private string GetTooltip()
	{
		string headerLocKey = (_enabled ? HideGridRenderingLocKey : ShowGridRenderingLocKey);
		return _keyBindingTooltipFactory.Create(headerLocKey, ToggleGuidelinesKey, ShowGuidelinesKey);
	}
}

using Timberborn.CoreUI;
using Timberborn.InputSystemUI;
using Timberborn.MapEditorNaturalResources;
using Timberborn.SingletonSystem;
using Timberborn.TooltipSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapEditorNaturalResourcesUI;

internal class NaturalResourceLayerToggle : ILoadableSingleton
{
	private static readonly string NaturalResourceLayerClass = "square-toggle--natural-resources";

	private static readonly string ShowNaturalResourcesLocKey = "NaturalResources.Visibility.Show";

	private static readonly string HideNaturalResourcesLocKey = "NaturalResources.Visibility.Hide";

	private static readonly string ToggleNaturalResourcesKey = "ToggleNaturalResources";

	private readonly NaturalResourceLayerService _naturalResourceLayerService;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly UILayout _uiLayout;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly BindableToggleFactory _bindableToggleFactory;

	private readonly EventBus _eventBus;

	private BindableToggle _bindableToggle;

	private string TooltipLocKey
	{
		get
		{
			if (!_naturalResourceLayerService.Enabled)
			{
				return ShowNaturalResourcesLocKey;
			}
			return HideNaturalResourcesLocKey;
		}
	}

	public NaturalResourceLayerToggle(NaturalResourceLayerService naturalResourceLayerService, VisualElementLoader visualElementLoader, UILayout uiLayout, ITooltipRegistrar tooltipRegistrar, BindableToggleFactory bindableToggleFactory, EventBus eventBus)
	{
		_naturalResourceLayerService = naturalResourceLayerService;
		_visualElementLoader = visualElementLoader;
		_uiLayout = uiLayout;
		_tooltipRegistrar = tooltipRegistrar;
		_bindableToggleFactory = bindableToggleFactory;
		_eventBus = eventBus;
	}

	public void Load()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/SquareToggle");
		_tooltipRegistrar.RegisterLocalizable(visualElement, () => TooltipLocKey);
		Toggle toggle = visualElement.Q<Toggle>("Toggle");
		toggle.AddToClassList(NaturalResourceLayerClass);
		_bindableToggle = _bindableToggleFactory.CreateAndBind(toggle, ToggleNaturalResourcesKey, ToggleNaturalResourcesLayer, () => _naturalResourceLayerService.Enabled);
		_uiLayout.AddTopRightButton(visualElement, 3);
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnNaturalResourceLayerChanged(NaturalResourceLayerChangedEvent naturalResourceLayerChangedEvent)
	{
		_bindableToggle.Update();
	}

	private void ToggleNaturalResourcesLayer(bool enableResources)
	{
		if (enableResources)
		{
			_naturalResourceLayerService.Enable();
		}
		else
		{
			_naturalResourceLayerService.Disable();
		}
	}
}

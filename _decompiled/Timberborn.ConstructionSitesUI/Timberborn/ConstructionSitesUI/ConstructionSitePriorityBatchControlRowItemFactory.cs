using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.BuilderPrioritySystem;
using Timberborn.BuilderPrioritySystemUI;
using Timberborn.Common;
using Timberborn.ConstructionSites;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.PrioritySystem;
using Timberborn.PrioritySystemUI;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.ConstructionSitesUI;

public class ConstructionSitePriorityBatchControlRowItemFactory
{
	private static readonly string TitleLocKey = "ConstructionSites.PriorityTitle";

	private readonly BuilderPrioritySpriteLoader _builderPrioritySpriteLoader;

	private readonly ILoc _loc;

	private readonly PriorityColors _priorityColors;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly VisualElementLoader _visualElementLoader;

	public ConstructionSitePriorityBatchControlRowItemFactory(BuilderPrioritySpriteLoader builderPrioritySpriteLoader, ILoc loc, PriorityColors priorityColors, ITooltipRegistrar tooltipRegistrar, VisualElementLoader visualElementLoader)
	{
		_builderPrioritySpriteLoader = builderPrioritySpriteLoader;
		_loc = loc;
		_priorityColors = priorityColors;
		_tooltipRegistrar = tooltipRegistrar;
		_visualElementLoader = visualElementLoader;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		ConstructionSite component = entity.GetComponent<ConstructionSite>();
		if (component != null)
		{
			BuilderPrioritizable builderPrioritizable = ((BaseComponent)(object)component).GetComponent<BuilderPrioritizable>();
			string elementName = "Game/BatchControl/PrioritizableBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			Image image = visualElement.Q<Image>("Priority");
			_tooltipRegistrar.Register(image, () => GetTooltipText(builderPrioritizable));
			visualElement.Q<Button>("Increase").RegisterCallback<ClickEvent>(delegate
			{
				IncreasePriority(builderPrioritizable);
			});
			visualElement.Q<Button>("Decrease").RegisterCallback<ClickEvent>(delegate
			{
				DecreasePriority(builderPrioritizable);
			});
			return new ConstructionSitePriorityBatchControlRowItem(_builderPrioritySpriteLoader, visualElement, builderPrioritizable, component, image, _priorityColors);
		}
		return null;
	}

	private static void IncreasePriority(BuilderPrioritizable builderPrioritizable)
	{
		Priority priority = builderPrioritizable.Priority;
		builderPrioritizable.SetPriority(priority.Next());
	}

	private static void DecreasePriority(BuilderPrioritizable builderPrioritizable)
	{
		Priority priority = builderPrioritizable.Priority;
		builderPrioritizable.SetPriority(priority.Previous());
	}

	private string GetTooltipText(BuilderPrioritizable builderPrioritizable)
	{
		return _loc.T(TitleLocKey) + " " + _loc.T(builderPrioritizable.Priority.GetLocKey());
	}
}

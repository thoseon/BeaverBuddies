using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.PrioritySystem;
using Timberborn.PrioritySystemUI;
using Timberborn.TooltipSystem;
using Timberborn.WorkSystem;
using UnityEngine.UIElements;

namespace Timberborn.WorkSystemUI;

public class WorkplacePriorityBatchControlRowItemFactory
{
	private static readonly string TitleLocKey = "Work.PriorityTitle";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly WorkplacePrioritySpriteLoader _workplacePrioritySpriteLoader;

	private readonly PriorityColors _priorityColors;

	private readonly ILoc _loc;

	public WorkplacePriorityBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, WorkplacePrioritySpriteLoader workplacePrioritySpriteLoader, PriorityColors priorityColors, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_workplacePrioritySpriteLoader = workplacePrioritySpriteLoader;
		_priorityColors = priorityColors;
		_loc = loc;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		WorkplacePriority workplacePriority = entity.GetComponent<WorkplacePriority>();
		if (workplacePriority != null)
		{
			string elementName = "Game/BatchControl/PrioritizableBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			Image image = visualElement.Q<Image>("Priority");
			_tooltipRegistrar.Register(image, () => GetTitle(workplacePriority));
			visualElement.Q<Button>("Increase").RegisterCallback<ClickEvent>(delegate
			{
				IncreasePriority(workplacePriority);
			});
			visualElement.Q<Button>("Decrease").RegisterCallback<ClickEvent>(delegate
			{
				DecreasePriority(workplacePriority);
			});
			return new WorkplacePriorityBatchControlRowItem(visualElement, workplacePriority, image, _workplacePrioritySpriteLoader, _priorityColors);
		}
		return null;
	}

	private string GetTitle(WorkplacePriority workplacePriority)
	{
		return _loc.T(TitleLocKey) + ": " + _loc.T(workplacePriority.Priority.GetLocKey());
	}

	private static void IncreasePriority(WorkplacePriority workplacePriority)
	{
		Priority priority = workplacePriority.Priority;
		workplacePriority.SetPriority(priority.Next());
	}

	private static void DecreasePriority(WorkplacePriority workplacePriority)
	{
		Priority priority = workplacePriority.Priority;
		workplacePriority.SetPriority(priority.Previous());
	}
}

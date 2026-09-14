using Timberborn.PrioritySystemUI;
using UnityEngine.UIElements;

namespace Timberborn.WorkSystemUI;

internal class WorkplacePriorityToggleGroupFactory
{
	private static readonly string DecreaseWorkplacePriorityKey = "DecreaseWorkplacePriority";

	private static readonly string IncreaseWorkplacePriorityKey = "IncreaseWorkplacePriority";

	private readonly PriorityToggleGroupFactory _priorityToggleGroupFactory;

	private readonly WorkplacePrioritySpriteLoader _workplacePrioritySpriteLoader;

	public WorkplacePriorityToggleGroupFactory(PriorityToggleGroupFactory priorityToggleGroupFactory, WorkplacePrioritySpriteLoader workplacePrioritySpriteLoader)
	{
		_priorityToggleGroupFactory = priorityToggleGroupFactory;
		_workplacePrioritySpriteLoader = workplacePrioritySpriteLoader;
	}

	public PriorityToggleGroup Create(VisualElement parent, string labelLocKey)
	{
		return _priorityToggleGroupFactory.Create(parent, labelLocKey, _workplacePrioritySpriteLoader, DecreaseWorkplacePriorityKey, IncreaseWorkplacePriorityKey);
	}
}

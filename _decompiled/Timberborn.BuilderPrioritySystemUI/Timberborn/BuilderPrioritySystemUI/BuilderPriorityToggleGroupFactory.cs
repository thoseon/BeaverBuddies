using Timberborn.PrioritySystemUI;
using UnityEngine.UIElements;

namespace Timberborn.BuilderPrioritySystemUI;

public class BuilderPriorityToggleGroupFactory
{
	private static readonly string DecreaseBuildersPriorityKey = "DecreaseBuildersPriority";

	private static readonly string IncreaseBuildersPriorityKey = "IncreaseBuildersPriority";

	private readonly BuilderPrioritySpriteLoader _builderPrioritySpriteLoader;

	private readonly PriorityToggleGroupFactory _priorityToggleGroupFactory;

	public BuilderPriorityToggleGroupFactory(BuilderPrioritySpriteLoader builderPrioritySpriteLoader, PriorityToggleGroupFactory priorityToggleGroupFactory)
	{
		_builderPrioritySpriteLoader = builderPrioritySpriteLoader;
		_priorityToggleGroupFactory = priorityToggleGroupFactory;
	}

	public PriorityToggleGroup Create(VisualElement parent, string labelLocKey)
	{
		return _priorityToggleGroupFactory.Create(parent, labelLocKey, _builderPrioritySpriteLoader, DecreaseBuildersPriorityKey, IncreaseBuildersPriorityKey);
	}
}

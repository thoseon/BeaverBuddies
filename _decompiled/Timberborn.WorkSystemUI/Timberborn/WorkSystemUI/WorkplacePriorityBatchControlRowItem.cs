using Timberborn.BatchControl;
using Timberborn.PrioritySystem;
using Timberborn.PrioritySystemUI;
using Timberborn.WorkSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.WorkSystemUI;

internal class WorkplacePriorityBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private readonly Image _image;

	private readonly WorkplacePriority _workplacePriority;

	private readonly WorkplacePrioritySpriteLoader _workplacePrioritySpriteLoader;

	private readonly PriorityColors _priorityColors;

	public VisualElement Root { get; }

	public WorkplacePriorityBatchControlRowItem(VisualElement root, WorkplacePriority workplacePriority, Image image, WorkplacePrioritySpriteLoader workplacePrioritySpriteLoader, PriorityColors priorityColors)
	{
		Root = root;
		_workplacePriority = workplacePriority;
		_image = image;
		_workplacePrioritySpriteLoader = workplacePrioritySpriteLoader;
		_priorityColors = priorityColors;
	}

	public void UpdateRowItem()
	{
		Priority priority = _workplacePriority.Priority;
		Sprite v = _workplacePrioritySpriteLoader.LoadSprite(priority);
		Color buttonColor = _priorityColors.GetButtonColor(priority);
		_image.style.backgroundImage = new StyleBackground(v);
		_image.style.unityBackgroundImageTintColor = buttonColor;
	}
}

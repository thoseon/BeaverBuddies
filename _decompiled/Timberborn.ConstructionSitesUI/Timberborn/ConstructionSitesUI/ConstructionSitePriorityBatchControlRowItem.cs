using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.BuilderPrioritySystem;
using Timberborn.BuilderPrioritySystemUI;
using Timberborn.ConstructionSites;
using Timberborn.CoreUI;
using Timberborn.PrioritySystem;
using Timberborn.PrioritySystemUI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.ConstructionSitesUI;

internal class ConstructionSitePriorityBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private readonly BuilderPrioritySpriteLoader _builderPrioritySpriteLoader;

	private readonly Image _image;

	private readonly ConstructionSite _constructionSite;

	private readonly BuilderPrioritizable _builderPrioritizable;

	private readonly PriorityColors _priorityColors;

	public VisualElement Root { get; }

	public ConstructionSitePriorityBatchControlRowItem(BuilderPrioritySpriteLoader builderPrioritySpriteLoader, VisualElement root, BuilderPrioritizable builderPrioritizable, ConstructionSite constructionSite, Image image, PriorityColors priorityColors)
	{
		_builderPrioritySpriteLoader = builderPrioritySpriteLoader;
		Root = root;
		_builderPrioritizable = builderPrioritizable;
		_constructionSite = constructionSite;
		_image = image;
		_priorityColors = priorityColors;
	}

	public void UpdateRowItem()
	{
		if (((BaseComponent)(object)_constructionSite).Enabled)
		{
			Root.ToggleDisplayStyle(visible: true);
			Priority priority = _builderPrioritizable.Priority;
			Sprite v = _builderPrioritySpriteLoader.LoadSprite(priority);
			Color buttonColor = _priorityColors.GetButtonColor(priority);
			_image.style.backgroundImage = new StyleBackground(v);
			_image.style.unityBackgroundImageTintColor = buttonColor;
		}
		else
		{
			Root.ToggleDisplayStyle(visible: false);
		}
	}
}

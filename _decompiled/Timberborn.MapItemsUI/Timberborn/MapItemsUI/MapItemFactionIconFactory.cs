using Timberborn.CoreUI;
using Timberborn.FactionSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.MapItemsUI;

public class MapItemFactionIconFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	public MapItemFactionIconFactory(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement Create(FactionSpec factionSpec)
	{
		VisualElement visualElement = CreateEmpty();
		Sprite asset = factionSpec.Logo.Asset;
		visualElement.style.backgroundImage = new StyleBackground(asset);
		return visualElement;
	}

	public VisualElement CreateEmpty()
	{
		return _visualElementLoader.LoadVisualElement("Common/MapItemFactionIcon");
	}
}

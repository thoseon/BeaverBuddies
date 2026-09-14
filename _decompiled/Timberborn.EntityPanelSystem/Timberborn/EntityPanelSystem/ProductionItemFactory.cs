using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.EntityPanelSystem;

public class ProductionItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	public ProductionItemFactory(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement CreateInputOutput(IEnumerable<VisualElement> inputs, IEnumerable<VisualElement> outputs, string craftingTime)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/EntityDescription/ProductionItem");
		visualElement.Q<VisualElement>("InputWrapper").Add(CreateInput(inputs));
		visualElement.Q<VisualElement>("OutputWrapper").Add(CreateOutput(outputs));
		Label label = visualElement.Q<Label>("CraftingTime");
		label.text = craftingTime;
		label.ToggleDisplayStyle(visible: true);
		return visualElement;
	}

	public VisualElement CreateInput(VisualElement input)
	{
		return CreateInput(Enumerables.One(input));
	}

	public VisualElement CreateOutput(VisualElement output)
	{
		return CreateOutput(Enumerables.One(output));
	}

	public VisualElement CreateInput(IEnumerable<VisualElement> inputs)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/EntityDescription/ProductionItemInput");
		Timberborn.CoreUI.VisualElementExtensions.ToggleDisplayStyle(visible: FillItems(visualElement, "Input", inputs), visualElement: visualElement.Q<VisualElement>("InputArrow"));
		return visualElement;
	}

	public VisualElement CreateOutput(IEnumerable<VisualElement> outputs)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/EntityDescription/ProductionItemOutput");
		Timberborn.CoreUI.VisualElementExtensions.ToggleDisplayStyle(visible: FillItems(visualElement, "Output", outputs), visualElement: visualElement.Q<VisualElement>("OutputArrow"));
		return visualElement;
	}

	private static bool FillItems(VisualElement element, string rootName, IEnumerable<VisualElement> items)
	{
		VisualElement visualElement = element.Q<VisualElement>(rootName);
		foreach (VisualElement item in items)
		{
			visualElement.Add(item);
		}
		return visualElement.childCount > 0;
	}
}

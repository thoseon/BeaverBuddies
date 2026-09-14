using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using UnityEngine.UIElements;

namespace Timberborn.SliderToggleSystem;

public class SliderToggleFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly SliderToggleButtonFactory _sliderToggleButtonFactory;

	private readonly InputService _inputService;

	public SliderToggleFactory(VisualElementLoader visualElementLoader, SliderToggleButtonFactory sliderToggleButtonFactory, InputService inputService)
	{
		_visualElementLoader = visualElementLoader;
		_sliderToggleButtonFactory = sliderToggleButtonFactory;
		_inputService = inputService;
	}

	public SliderToggle Create(VisualElement parent, params SliderToggleItem[] items)
	{
		return CreateBindable(parent, null, items);
	}

	public SliderToggle CreateBindable(VisualElement parent, string toggleBindingKey, params SliderToggleItem[] items)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/SliderToggle");
		parent.Add(visualElement);
		return new SliderToggle(_inputService, visualElement, toggleBindingKey, CreateItems(visualElement.Q<VisualElement>("Content"), items));
	}

	private IEnumerable<SliderToggleButton> CreateItems(VisualElement parent, IReadOnlyList<SliderToggleItem> items)
	{
		for (int i = 0; i < items.Count; i++)
		{
			yield return _sliderToggleButtonFactory.Create(parent, items[i]);
		}
	}
}

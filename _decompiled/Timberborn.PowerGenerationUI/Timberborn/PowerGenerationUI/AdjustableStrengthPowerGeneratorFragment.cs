using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.PowerGeneration;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.PowerGenerationUI;

internal class AdjustableStrengthPowerGeneratorFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly IntegerSliderFactory _integerSliderFactory;

	private AdjustableStrengthPowerGenerator _generator;

	private VisualElement _root;

	private VisualElement _sliderRoot;

	public AdjustableStrengthPowerGeneratorFragment(VisualElementLoader visualElementLoader, IntegerSliderFactory integerSliderFactory)
	{
		_visualElementLoader = visualElementLoader;
		_integerSliderFactory = integerSliderFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/AdjustableStrengthPowerGeneratorFragment");
		_sliderRoot = _root.Q<VisualElement>("SliderRoot");
		_root.Q<Button>("FlipRotation").RegisterCallback<ClickEvent>(delegate
		{
			FlipRotation();
		});
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_generator = entity.GetComponent<AdjustableStrengthPowerGenerator>();
		if ((bool)_generator)
		{
			_sliderRoot.Add(CreateSlider());
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	public void ClearFragment()
	{
		if ((bool)_generator)
		{
			_sliderRoot.Clear();
			_generator = null;
		}
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
	}

	private void FlipRotation()
	{
		if ((bool)_generator)
		{
			_generator.FlipRotation();
		}
	}

	private VisualElement CreateSlider()
	{
		int current = Mathf.RoundToInt(_generator.GeneratorStrength * (float)_generator.MaxValue);
		return _integerSliderFactory.Create(current, _generator.MaxValue, ChangeValue);
	}

	private void ChangeValue(int newValue)
	{
		_generator.GeneratorStrength = (float)newValue / (float)_generator.MaxValue;
	}
}

using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class IntegerSliderFactory
{
	private static readonly int WidthPerDigit = 8;

	private readonly VisualElementLoader _visualElementLoader;

	public IntegerSliderFactory(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement Create(int current, int max, Action<int> callback)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/IntegerSlider");
		Slider slider = visualElement.Q<Slider>("Slider");
		slider.value = (float)current / (float)max;
		IntegerField integerField = visualElement.Q<IntegerField>("Value");
		visualElement.Q<Label>("MaxValue").text = max.ToString();
		int num = max.ToString().Length * WidthPerDigit;
		integerField.Q<TextElement>().style.width = num;
		slider.RegisterValueChangedCallback(delegate(ChangeEvent<float> changeEvent)
		{
			ChangeValue(changeEvent, integerField, slider, max, callback);
		});
		TextFields.InitializeIntegerField(integerField, current, 0, max, delegate(int newValue)
		{
			ChangeValue(newValue, integerField, slider, max, callback);
		});
		return visualElement;
	}

	private static void ChangeValue(ChangeEvent<float> changeEvent, IntegerField integerField, Slider slider, int max, Action<int> callback)
	{
		ChangeValue(Mathf.RoundToInt(changeEvent.newValue * (float)max), integerField, slider, max, callback);
	}

	private static void ChangeValue(int newValue, IntegerField integerField, Slider slider, int max, Action<int> callback)
	{
		newValue = Mathf.Clamp(newValue, 0, max);
		RefreshUI(integerField, slider, newValue, max);
		callback(newValue);
	}

	private static void RefreshUI(IntegerField integerField, Slider slider, int current, int max)
	{
		integerField.SetValueWithoutNotify(current);
		slider.SetValueWithoutNotify((float)current / (float)max);
	}
}

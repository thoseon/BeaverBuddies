using System;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public static class TextFields
{
	public static void InitializeIntegerField(IntegerField integerField, int startingValue, int minValue = 0, int maxValue = int.MaxValue, Action<int> afterEditingCallback = null)
	{
		integerField.SetValueWithoutNotify(startingValue);
		integerField.RegisterCallback<FocusOutEvent>(delegate
		{
			int valueWithoutNotify = Math.Clamp(integerField.value, minValue, maxValue);
			integerField.SetValueWithoutNotify(valueWithoutNotify);
			afterEditingCallback?.Invoke(integerField.value);
		});
	}

	public static void InitializeFloatField(FloatField floatField, float startingValue, float minValue = float.MinValue, float maxValue = float.MaxValue, Action<float> afterEditingCallback = null)
	{
		floatField.SetValueWithoutNotify(startingValue);
		floatField.RegisterValueChangedCallback(delegate(ChangeEvent<float> evt)
		{
			float valueWithoutNotify = Math.Clamp(evt.newValue, minValue, maxValue);
			floatField.SetValueWithoutNotify(valueWithoutNotify);
			afterEditingCallback?.Invoke(floatField.value);
		});
	}
}

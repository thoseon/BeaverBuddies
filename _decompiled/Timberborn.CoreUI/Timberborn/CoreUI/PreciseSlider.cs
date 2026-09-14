using System;
using System.Runtime.CompilerServices;
using Timberborn.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

[UxmlElement]
public class PreciseSlider : VisualElement
{
	[Serializable]
	[CompilerGenerated]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		public override object CreateInstance()
		{
			return new PreciseSlider();
		}
	}

	private readonly Slider _slider;

	private readonly VisualElement _marker;

	private Action<float> _valueChangedCallback;

	private float _step = 1f;

	private float? _markerValue;

	private bool _suppressCallback;

	public float Value { get; private set; }

	public PreciseSlider()
	{
		VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("UI/Views/Core/PreciseSlider");
		VisualTreeAsset visualTreeAsset2 = Resources.Load<VisualTreeAsset>("UI/Views/Core/PreciseSliderMarker");
		visualTreeAsset.CloneTree(this);
		_slider = this.Q<Slider>("Slider");
		_slider.RegisterValueChangedCallback(OnSliderValueChanged);
		this.Q<Button>("DecreaseButton").RegisterCallback<ClickEvent>(OnDecreaseClicked);
		this.Q<Button>("IncreaseButton").RegisterCallback<ClickEvent>(OnIncreaseClicked);
		visualTreeAsset2.CloneTree(_slider.Q("unity-drag-container"));
		_marker = _slider.Q("PreciseSliderMarker");
		_marker.PlaceBehind(_slider.Q("unity-dragger"));
		_marker.ToggleDisplayStyle(visible: false);
		_slider.RegisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
		_marker.RegisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
	}

	public void SetValueChangedCallback(Action<float> valueChangedCallback)
	{
		_valueChangedCallback = valueChangedCallback;
	}

	public void UpdateValuesWithoutNotify(float value, float maxValue)
	{
		UpdateValuesWithoutNotify(value, 0f, maxValue);
	}

	public void UpdateValuesWithoutNotify(float value, float minValue, float maxValue)
	{
		bool flag = false;
		_suppressCallback = true;
		if (!_slider.lowValue.Equals(minValue))
		{
			_slider.lowValue = minValue;
			flag = true;
		}
		if (!_slider.highValue.Equals(maxValue))
		{
			_slider.highValue = maxValue;
			flag = true;
		}
		_suppressCallback = false;
		SetValueWithoutNotify(value);
		if (flag)
		{
			UpdateMarker();
		}
	}

	public void SetValueWithoutNotify(float value)
	{
		UpdateValue(value, _slider.value, invokeCallback: false);
	}

	public void SetStepWithoutNotify(float step)
	{
		_step = step;
		UpdateValue(Value, Value, invokeCallback: false);
	}

	public void SetMarker(float value)
	{
		if (!_markerValue.Equals(value))
		{
			_markerValue = value;
			UpdateMarker();
		}
	}

	public void ClearMarker()
	{
		if (_markerValue.HasValue)
		{
			_markerValue = null;
			UpdateMarker();
		}
	}

	private void OnDecreaseClicked(ClickEvent evt)
	{
		_slider.value -= _step;
	}

	private void OnIncreaseClicked(ClickEvent evt)
	{
		_slider.value += _step;
	}

	private void OnSliderValueChanged(ChangeEvent<float> evt)
	{
		if (!_suppressCallback)
		{
			UpdateValue(evt.newValue, evt.previousValue, invokeCallback: true);
		}
	}

	private void OnGeometryChangedEvent(GeometryChangedEvent evt)
	{
		UpdateMarker();
	}

	private void UpdateValue(float value, float previousValue, bool invokeCallback)
	{
		Value = Numbers.RoundToPrecision(value, _step);
		if (Math.Abs(Value - previousValue) > Mathf.Epsilon)
		{
			_slider.SetValueWithoutNotify(value);
			if (invokeCallback)
			{
				_valueChangedCallback?.Invoke(Value);
			}
		}
	}

	private void UpdateMarker()
	{
		float num = _slider.highValue - _slider.lowValue;
		if (_markerValue.HasValue && num != 0f)
		{
			_marker.style.translate = new Translate(Length.Pixels(Mathf.Clamp01((_markerValue.Value - _slider.lowValue) / num) * (_slider.contentRect.width - _marker.layout.width)), Length.Pixels(0f));
			_marker.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_marker.ToggleDisplayStyle(visible: false);
		}
	}
}

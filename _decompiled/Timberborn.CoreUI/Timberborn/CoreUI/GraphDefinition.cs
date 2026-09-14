using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.CoreUI;

public class GraphDefinition
{
	private readonly List<float> _dataPoints = new List<float>();

	public Color Color { get; }

	public bool IsVisible { get; private set; }

	public float MaxValue { get; private set; }

	public float MinValue { get; private set; }

	public ReadOnlyList<float> DataPoints => _dataPoints.AsReadOnlyList();

	public GraphDefinition(Color color, bool isVisible = true)
	{
		Color = color;
		IsVisible = isVisible;
	}

	public void AddDataPoint(float value)
	{
		_dataPoints.Add(value);
		if (value > MaxValue)
		{
			MaxValue = value;
		}
		if (value < MinValue)
		{
			MinValue = value;
		}
	}

	public void Clear()
	{
		_dataPoints.Clear();
		MaxValue = 0f;
		MinValue = 0f;
	}

	public void SetVisibility(bool state)
	{
		IsVisible = state;
	}
}

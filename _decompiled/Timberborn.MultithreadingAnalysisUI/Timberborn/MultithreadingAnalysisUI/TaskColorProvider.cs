using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.MultithreadingAnalysis;
using UnityEngine;

namespace Timberborn.MultithreadingAnalysisUI;

internal class TaskColorProvider
{
	private static readonly Color[] PredefinedPalette = new Color[20]
	{
		new Color(0.51f, 0.2f, 0.2f),
		new Color(0.48f, 0.34f, 0.2f),
		new Color(0.45f, 0.4f, 0.2f),
		new Color(0.54f, 0.7f, 0.2f),
		new Color(0.2f, 0.65f, 0.3f),
		new Color(0.2f, 0.5f, 0.4f),
		new Color(0.22f, 0.42f, 0.72f),
		new Color(0.22f, 0.36f, 0.56f),
		new Color(0.3f, 0.2f, 0.56f),
		new Color(0.62f, 0.4f, 0.76f),
		new Color(0.52f, 0.2f, 0.46f),
		new Color(0.73f, 0.2f, 0.34f),
		new Color(0.4f, 0.26f, 0.26f),
		new Color(0.38f, 0.38f, 0.38f),
		new Color(0.22f, 0.32f, 0.22f),
		new Color(0.1f, 0.16f, 0.09f),
		new Color(0.38f, 0.38f, 0.52f),
		new Color(0.24f, 0.4f, 0.4f),
		new Color(0.36f, 0.52f, 0.36f),
		new Color(0.52f, 0.36f, 0.3f)
	};

	private readonly Dictionary<Type, Color> _colorMap = new Dictionary<Type, Color>();

	public void InitializeFromSamples(ReadOnlyList<TaskSample> samples)
	{
		_colorMap.Clear();
		foreach (Type item in from type in samples.Select((TaskSample sample) => sample.GenericType).Distinct()
			orderby type.Name
			select type)
		{
			_colorMap[item] = PredefinedPalette[_colorMap.Count % PredefinedPalette.Length];
		}
		if (_colorMap.Count > PredefinedPalette.Length)
		{
			Debug.LogWarning("Exceeded predefined task color palette, colors may repeat.");
		}
	}

	public Color GetColor(Type type)
	{
		return _colorMap[type];
	}
}

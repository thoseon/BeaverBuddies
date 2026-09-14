using System;
using UnityEngine;

namespace Timberborn.PrefabOptimization;

internal class ReusableColorArray
{
	private Color32[] _array;

	private Color32 _filledColor;

	private int _filledLength;

	public Color32[] Get(int minLength, Color32 color)
	{
		if (_array == null || _array.Length < minLength)
		{
			_array = new Color32[minLength];
			_filledColor = default(Color32);
			_filledLength = _array.Length;
		}
		if (!ColorsAreEqual(_filledColor, color) || _filledLength < minLength)
		{
			Array.Fill(_array, color, 0, minLength);
			_filledColor = color;
			_filledLength = minLength;
		}
		return _array;
	}

	public void Clear()
	{
		_array = null;
	}

	private static bool ColorsAreEqual(Color32 a, Color32 b)
	{
		if (a.r == b.r && a.g == b.g && a.b == b.b)
		{
			return a.a == b.a;
		}
		return false;
	}
}

using System;
using UnityEngine;

namespace Timberborn.Rendering;

public readonly struct MaterialProperties : IEquatable<MaterialProperties>
{
	public Color Color { get; }

	public float Grayscale { get; }

	public Color LightingColor { get; }

	public MaterialProperties(Color color, float grayscale, Color lightingColor)
	{
		Color = color;
		Grayscale = grayscale;
		LightingColor = lightingColor;
	}

	public bool Equals(MaterialProperties other)
	{
		if (Color.Equals(other.Color) && Grayscale.Equals(other.Grayscale))
		{
			return LightingColor.Equals(other.LightingColor);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is MaterialProperties other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Color, Grayscale, LightingColor);
	}

	public static bool operator ==(MaterialProperties left, MaterialProperties right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(MaterialProperties left, MaterialProperties right)
	{
		return !left.Equals(right);
	}
}

using System;
using System.Text;
using Timberborn.Coordinates;

namespace Timberborn.TerrainSystemRendering;

public readonly struct SurfaceBlockShape : IEquatable<SurfaceBlockShape>
{
	private readonly RelativeHeight _height11;

	private readonly RelativeHeight _height10;

	private readonly RelativeHeight _height00;

	private readonly RelativeHeight _height01;

	public short Index { get; }

	public bool IsVisible
	{
		get
		{
			if (!FullyUnderground)
			{
				return !FullyAboveGround;
			}
			return false;
		}
	}

	private bool FullyUnderground
	{
		get
		{
			if ((_height11 == RelativeHeight.Lower || _height11 == RelativeHeight.Empty) && (_height10 == RelativeHeight.Lower || _height10 == RelativeHeight.Empty) && (_height00 == RelativeHeight.Lower || _height00 == RelativeHeight.Empty))
			{
				if (_height01 != RelativeHeight.Lower)
				{
					return _height01 == RelativeHeight.Empty;
				}
				return true;
			}
			return false;
		}
	}

	private bool FullyAboveGround
	{
		get
		{
			if ((_height11 == RelativeHeight.Higher || _height11 == RelativeHeight.Empty) && (_height10 == RelativeHeight.Higher || _height10 == RelativeHeight.Empty) && (_height00 == RelativeHeight.Higher || _height00 == RelativeHeight.Empty))
			{
				if (_height01 != RelativeHeight.Higher)
				{
					return _height01 == RelativeHeight.Empty;
				}
				return true;
			}
			return false;
		}
	}

	public SurfaceBlockShape(RelativeHeight height11, RelativeHeight height10, RelativeHeight height00, RelativeHeight height01)
	{
		_height11 = height11;
		_height10 = height10;
		_height00 = height00;
		_height01 = height01;
		Index = (short)((uint)_height00 | ((uint)_height01 << 3) | ((uint)_height10 << 6) | ((uint)_height11 << 9));
	}

	public static SurfaceBlockShape FromModelName(string modelName)
	{
		if (modelName.Length < 4 || (modelName.Length > 4 && modelName[4] != '-'))
		{
			throw new ArgumentException("Invalid model name: " + modelName, "modelName");
		}
		return new SurfaceBlockShape(RelativeHeightExtensions.FromModelNameCharacter(modelName[0]), RelativeHeightExtensions.FromModelNameCharacter(modelName[1]), RelativeHeightExtensions.FromModelNameCharacter(modelName[2]), RelativeHeightExtensions.FromModelNameCharacter(modelName[3]));
	}

	public SurfaceBlockShape Rotate(Orientation orientation)
	{
		return orientation switch
		{
			Orientation.Cw0 => this, 
			Orientation.Cw90 => new SurfaceBlockShape(_height01, _height11, _height10, _height00), 
			Orientation.Cw180 => new SurfaceBlockShape(_height00, _height01, _height11, _height10), 
			Orientation.Cw270 => new SurfaceBlockShape(_height10, _height00, _height01, _height11), 
			_ => throw new ArgumentOutOfRangeException("orientation", orientation, null), 
		};
	}

	public bool Equals(SurfaceBlockShape other)
	{
		if (_height11 == other._height11 && _height10 == other._height10 && _height00 == other._height00)
		{
			return _height01 == other._height01;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is SurfaceBlockShape other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (int)(((((uint)((int)_height11 * 397) ^ (uint)_height10) * 397) ^ (uint)_height00) * 397) ^ (int)_height01;
	}

	public override string ToString()
	{
		return ToModelName();
	}

	public static bool operator ==(SurfaceBlockShape left, SurfaceBlockShape right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(SurfaceBlockShape left, SurfaceBlockShape right)
	{
		return !left.Equals(right);
	}

	private string ToModelName()
	{
		StringBuilder stringBuilder = new StringBuilder(4);
		stringBuilder.Append(RelativeHeightExtensions.ToModelNameCharacter(_height11));
		stringBuilder.Append(RelativeHeightExtensions.ToModelNameCharacter(_height10));
		stringBuilder.Append(RelativeHeightExtensions.ToModelNameCharacter(_height00));
		stringBuilder.Append(RelativeHeightExtensions.ToModelNameCharacter(_height01));
		return stringBuilder.ToString();
	}
}

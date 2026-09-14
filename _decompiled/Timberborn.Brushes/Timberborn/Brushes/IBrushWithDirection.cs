namespace Timberborn.Brushes;

public interface IBrushWithDirection
{
	bool Increase { set; }

	bool Inverse { set; }

	bool IsIncreasing { get; }
}

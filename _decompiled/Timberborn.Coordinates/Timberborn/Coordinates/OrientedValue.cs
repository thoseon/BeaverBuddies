namespace Timberborn.Coordinates;

public readonly struct OrientedValue<T>
{
	public T Value { get; }

	public Orientation Orientation { get; }

	public OrientedValue(T value, Orientation orientation)
	{
		Value = value;
		Orientation = orientation;
	}

	public void Deconstruct(out T value, out Orientation orientation)
	{
		value = Value;
		orientation = Orientation;
	}
}

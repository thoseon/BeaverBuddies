namespace Timberborn.Common;

public readonly struct ReadOnlyJaggedArray<T>(T[][] array)
{
	private readonly T[][] _array = array;

	public ref readonly T Get(int row, int column)
	{
		return ref _array[row][column];
	}
}

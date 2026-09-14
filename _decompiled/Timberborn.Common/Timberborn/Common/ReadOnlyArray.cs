using System;

namespace Timberborn.Common;

public readonly struct ReadOnlyArray<T>(T[] array)
{
	private readonly T[] _array = array;

	public ReadOnlySpan<T> AsSpan => MemoryExtensions.AsSpan(_array);

	public ref readonly T this[int index] => ref _array[index];
}

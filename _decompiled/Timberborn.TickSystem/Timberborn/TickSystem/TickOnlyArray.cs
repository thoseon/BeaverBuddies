using System;

namespace Timberborn.TickSystem;

public class TickOnlyArray<T>
{
	private T[] _array;

	private readonly TickOnlyArrayService _tickOnlyArrayService;

	public TickOnlyArray(int size, TickOnlyArrayService tickOnlyArrayService)
	{
		_array = new T[size];
		_tickOnlyArrayService = tickOnlyArrayService;
	}

	public ReadOnlySpan<T> GetReadOnlySpan()
	{
		return GetSpan();
	}

	public Span<T> GetSpan()
	{
		if (_tickOnlyArrayService.AllowEdit)
		{
			return MemoryExtensions.AsSpan(_array);
		}
		throw new InvalidOperationException("Cannot access array outside of singleton Tick.");
	}

	public T[] GetArray()
	{
		if (_tickOnlyArrayService.AllowFullAccess)
		{
			return _array;
		}
		throw new InvalidOperationException("Cannot access array outside of singleton StartParallelTick.");
	}

	public void Resize(int newSize)
	{
		if (_tickOnlyArrayService.AllowEdit)
		{
			Array.Resize(ref _array, newSize);
			return;
		}
		throw new InvalidOperationException("Cannot resize array outside of singleton Tick.");
	}
}

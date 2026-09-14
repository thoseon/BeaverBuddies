using System;

namespace Timberborn.Common;

public class BufferedArray<T>
{
	private T[] _current;

	private T[] _buffered;

	public T[] Current => _current;

	public T[] Buffered => _buffered;

	public void Initialize(int size)
	{
		_current = new T[size];
		_buffered = new T[size];
	}

	public void Swap()
	{
		T[] buffered = _buffered;
		T[] current = _current;
		_current = buffered;
		_buffered = current;
	}

	public void Unify()
	{
		_current.CopyTo(_buffered, 0);
	}

	public void Fill(T value)
	{
		for (int i = 0; i < _current.Length; i++)
		{
			_current[i] = value;
		}
	}

	public void ResizeAndFill(int newSize, T value)
	{
		int num = _current.Length;
		Array.Resize(ref _current, newSize);
		Array.Resize(ref _buffered, newSize);
		for (int i = num; i < newSize; i++)
		{
			_current[i] = value;
			_buffered[i] = value;
		}
	}
}

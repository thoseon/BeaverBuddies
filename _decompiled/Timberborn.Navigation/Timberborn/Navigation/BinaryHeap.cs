using System;
using System.Collections.Generic;

namespace Timberborn.Navigation;

internal class BinaryHeap<TValue> where TValue : IOrderable<TValue>
{
	private readonly List<TValue> _nodes;

	private int _nextFreeIndex;

	public BinaryHeap(int initialCapacity = 0)
	{
		if (initialCapacity < 0)
		{
			throw new ArgumentOutOfRangeException(string.Format("{0} can't be negative but is: {1}", "initialCapacity", initialCapacity));
		}
		_nodes = new List<TValue>(initialCapacity);
	}

	public void Push(TValue value)
	{
		int num = _nextFreeIndex++;
		AddToNodes(value, num);
		HeapifyFromIndexToBeginning(num);
	}

	public TValue Peek()
	{
		if (IsEmpty())
		{
			throw new InvalidOperationException("Can't peek because it's empty");
		}
		return _nodes[0];
	}

	public TValue Pop()
	{
		if (IsEmpty())
		{
			throw new InvalidOperationException("Can't pop because it's empty");
		}
		TValue result = _nodes[0];
		DeleteRoot();
		return result;
	}

	public void Clear()
	{
		_nextFreeIndex = 0;
	}

	public bool IsEmpty()
	{
		return _nextFreeIndex == 0;
	}

	private void AddToNodes(TValue value, int index)
	{
		if (index == _nodes.Count)
		{
			_nodes.Add(value);
		}
		else
		{
			_nodes[index] = value;
		}
	}

	private void HeapifyFromIndexToBeginning(int startingIndex)
	{
		int num = startingIndex;
		while (num > 0)
		{
			int num2 = (num - 1) / 2;
			if (NodeAtIsSmallerThanNodeAt(num2, num))
			{
				break;
			}
			SwapNodes(num2, num);
			num = num2;
		}
	}

	private void DeleteRoot()
	{
		int index = _nextFreeIndex - 1;
		_nodes[0] = _nodes[index];
		_nextFreeIndex--;
		HeapifyFromIndexToEnd(0);
	}

	private void HeapifyFromIndexToEnd(int startingIndex)
	{
		int num = startingIndex;
		if (!IndexIsInBounds(num))
		{
			return;
		}
		while (true)
		{
			int num2 = num;
			int num3 = 2 * num + 1;
			int num4 = 2 * num + 2;
			if (ChildAtIsSmallerThanNodeAt(num3, num2))
			{
				num2 = num3;
			}
			if (ChildAtIsSmallerThanNodeAt(num4, num2))
			{
				num2 = num4;
			}
			if (num2 == num)
			{
				break;
			}
			SwapNodes(num2, num);
			num = num2;
		}
		bool ChildAtIsSmallerThanNodeAt(int childIndex, int nodeIndex)
		{
			if (IndexIsInBounds(childIndex))
			{
				return NodeAtIsSmallerThanNodeAt(childIndex, nodeIndex);
			}
			return false;
		}
	}

	private bool IndexIsInBounds(int index)
	{
		return index < _nextFreeIndex;
	}

	private bool NodeAtIsSmallerThanNodeAt(int index1, int index2)
	{
		return _nodes[index1].IsLessThan(_nodes[index2]);
	}

	private void SwapNodes(int index1, int index2)
	{
		List<TValue> nodes = _nodes;
		List<TValue> nodes2 = _nodes;
		TValue value = _nodes[index2];
		TValue value2 = _nodes[index1];
		nodes[index1] = value;
		nodes2[index2] = value2;
	}
}

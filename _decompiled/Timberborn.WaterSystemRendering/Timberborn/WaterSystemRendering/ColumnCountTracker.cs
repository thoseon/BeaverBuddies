using System;

namespace Timberborn.WaterSystemRendering;

internal class ColumnCountTracker
{
	private int _lastMaxCount;

	public int MaxCount { get; private set; }

	public void Update(int maxIndex)
	{
		int lastMaxCount = _lastMaxCount;
		_lastMaxCount = maxIndex;
		MaxCount = Math.Max(lastMaxCount, _lastMaxCount);
	}
}

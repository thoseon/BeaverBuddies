namespace Timberborn.WaterSystemRendering;

internal class ColumnChangeTracker
{
	private static readonly int FrameCapacity = 3;

	private readonly bool[] _frameStorage = new bool[FrameCapacity];

	public void Update(bool anyColumnChanged)
	{
		for (int num = FrameCapacity - 1; num > 0; num--)
		{
			_frameStorage[num] = _frameStorage[num - 1];
		}
		_frameStorage[0] = anyColumnChanged;
	}

	public bool AnyColumnChanged()
	{
		for (int i = 0; i < FrameCapacity; i++)
		{
			if (_frameStorage[i])
			{
				return true;
			}
		}
		return false;
	}
}

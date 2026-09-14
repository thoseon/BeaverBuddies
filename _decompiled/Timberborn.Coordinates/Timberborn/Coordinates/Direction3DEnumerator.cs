namespace Timberborn.Coordinates;

public struct Direction3DEnumerator
{
	private readonly Directions3D _directions;

	private int _index;

	public Direction3D Current { get; private set; }

	public Direction3DEnumerator(Directions3D directions)
	{
		Current = Direction3D.Down;
		_directions = directions;
		_index = -1;
	}

	public Direction3DEnumerator GetEnumerator()
	{
		return this;
	}

	public bool MoveNext()
	{
		while (++_index < 6)
		{
			switch (_index)
			{
			case 0:
				if (_directions.HasFlag(Directions3D.Down))
				{
					Current = Direction3D.Down;
					return true;
				}
				break;
			case 1:
				if (_directions.HasFlag(Directions3D.Left))
				{
					Current = Direction3D.Left;
					return true;
				}
				break;
			case 2:
				if (_directions.HasFlag(Directions3D.Up))
				{
					Current = Direction3D.Up;
					return true;
				}
				break;
			case 3:
				if (_directions.HasFlag(Directions3D.Right))
				{
					Current = Direction3D.Right;
					return true;
				}
				break;
			case 4:
				if (_directions.HasFlag(Directions3D.Bottom))
				{
					Current = Direction3D.Bottom;
					return true;
				}
				break;
			case 5:
				if (_directions.HasFlag(Directions3D.Top))
				{
					Current = Direction3D.Top;
					return true;
				}
				break;
			}
		}
		return false;
	}
}

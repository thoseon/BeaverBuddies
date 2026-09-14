using System;
using UnityEngine;

namespace Timberborn.Common;

public readonly struct BoundingBox
{
	public struct Builder
	{
		private int _minX;

		private int _minY;

		private int _minZ;

		private int _maxX;

		private int _maxY;

		private int _maxZ;

		private bool _expandedAtLeastOnce;

		public void Expand(Vector3Int point)
		{
			int x = point.x;
			int y = point.y;
			int z = point.z;
			if (!_expandedAtLeastOnce)
			{
				_minX = x;
				_minY = y;
				_minZ = z;
				_maxX = x;
				_maxY = y;
				_maxZ = z;
				_expandedAtLeastOnce = true;
				return;
			}
			if (x < _minX)
			{
				_minX = x;
			}
			else if (x > _maxX)
			{
				_maxX = x;
			}
			if (y < _minY)
			{
				_minY = y;
			}
			else if (y > _maxY)
			{
				_maxY = y;
			}
			if (z < _minZ)
			{
				_minZ = z;
			}
			else if (z > _maxZ)
			{
				_maxZ = z;
			}
		}

		public BoundingBox Build()
		{
			if (!_expandedAtLeastOnce)
			{
				throw new InvalidOperationException("BoundingBox is empty");
			}
			return new BoundingBox(_minX, _minY, _minZ, _maxX, _maxY, _maxZ);
		}
	}

	private readonly int _minX;

	private readonly int _minY;

	private readonly int _minZ;

	private readonly int _maxX;

	private readonly int _maxY;

	private readonly int _maxZ;

	private BoundingBox(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
	{
		_minX = minX;
		_minY = minY;
		_minZ = minZ;
		_maxX = maxX;
		_maxY = maxY;
		_maxZ = maxZ;
	}

	public bool Contains(Vector3Int coordinates)
	{
		if (coordinates.x >= _minX && coordinates.x <= _maxX && coordinates.y >= _minY && coordinates.y <= _maxY && coordinates.z >= _minZ)
		{
			return coordinates.z <= _maxZ;
		}
		return false;
	}

	public bool Intersects(in BoundingBox boundingBox)
	{
		return !Disconnected(in boundingBox);
	}

	private bool Disconnected(in BoundingBox boundingBox)
	{
		if (_minX <= boundingBox._maxX && _maxX >= boundingBox._minX && _minY <= boundingBox._maxY && _maxY >= boundingBox._minY && _minZ <= boundingBox._maxZ)
		{
			return _maxZ < boundingBox._minZ;
		}
		return true;
	}
}

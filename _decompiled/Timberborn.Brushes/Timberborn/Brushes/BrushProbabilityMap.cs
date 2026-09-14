using Timberborn.Common;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.Brushes;

public class BrushProbabilityMap : ILoadableSingleton
{
	private readonly MapSize _mapSize;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private Vector3Int _size;

	private float[,] _probabilities;

	public BrushProbabilityMap(MapSize mapSize, IRandomNumberGenerator randomNumberGenerator)
	{
		_mapSize = mapSize;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Load()
	{
		_size = _mapSize.TerrainSize;
		_probabilities = new float[_size.x, _size.y];
		Reset();
	}

	public bool TestProbabilityAtCoordinates(Vector2Int coordinates, float density)
	{
		if (Sizing.SizeContains(_size, coordinates))
		{
			return _probabilities[coordinates.x, coordinates.y] <= density;
		}
		return false;
	}

	private void Reset()
	{
		for (int i = 0; i < _size.y; i++)
		{
			for (int j = 0; j < _size.x; j++)
			{
				_probabilities[j, i] = _randomNumberGenerator.Range(0f, 1f);
			}
		}
	}
}

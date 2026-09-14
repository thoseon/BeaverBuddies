using Timberborn.Common;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.TubeSystem;

internal class TubeMap : ILoadableSingleton
{
	private readonly MapSize _mapSize;

	private Array3D<Tube> _tubeMap;

	public bool AnyTubeBuilt { get; private set; }

	public TubeMap(MapSize mapSize)
	{
		_mapSize = mapSize;
	}

	public void Load()
	{
		_tubeMap = new Array3D<Tube>(_mapSize.TotalSize, () => (Tube)null);
	}

	public void SetTube(Tube tube, Vector3Int coordinates)
	{
		_tubeMap.GetRefAt(coordinates) = tube;
		AnyTubeBuilt = true;
	}

	public void UnsetTube(Vector3Int coordinates)
	{
		_tubeMap.GetRefAt(coordinates) = null;
	}

	public Tube GetTubeAt(Vector3Int gridPosition)
	{
		if (!_tubeMap.Contains(gridPosition))
		{
			return null;
		}
		return _tubeMap.GetRefAt(gridPosition);
	}
}

using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.WaterBuildings;

internal class PreviewWaterInputPipeBlockerService
{
	private readonly EventBus _eventBus;

	private readonly HashSet<Vector3Int> _blockedTiles = new HashSet<Vector3Int>();

	private readonly List<Vector3Int> _coordinatesCache = new List<Vector3Int>();

	public PreviewWaterInputPipeBlockerService(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Block(IEnumerable<Vector3Int> coordinates)
	{
		_coordinatesCache.AddRange(coordinates);
		foreach (Vector3Int item in _coordinatesCache)
		{
			_blockedTiles.Add(item);
		}
		PostChangeEvent(_coordinatesCache);
		_coordinatesCache.Clear();
	}

	public void Unblock(IEnumerable<Vector3Int> coordinates)
	{
		_coordinatesCache.AddRange(coordinates);
		foreach (Vector3Int item in _coordinatesCache)
		{
			_blockedTiles.Remove(item);
		}
		PostChangeEvent(_coordinatesCache);
		_coordinatesCache.Clear();
	}

	public bool IsBlocked(Vector3Int coordinates)
	{
		return _blockedTiles.Contains(coordinates);
	}

	private void PostChangeEvent(List<Vector3Int> coordinates)
	{
		_eventBus.Post(new PreviewBlockingCoordinatesChangedEvent(coordinates.AsReadOnlyList()));
	}
}

using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.Explosions;

public class ExplosionData
{
	private Dictionary<int, HashSet<Vector3Int>> _affectedTiles = new Dictionary<int, HashSet<Vector3Int>>();

	public float Radius { get; }

	public Vector3 Center { get; }

	public int CurrentExplosionRadius { get; private set; }

	public ExplosionData(float radius, Vector3 center, int currentExplosionRadius = 0)
	{
		Radius = radius;
		Center = center;
		CurrentExplosionRadius = currentExplosionRadius;
	}

	public void InitializeAffectedTiles(ExplosionOutcomeGatherer outcomeGatherer)
	{
		_affectedTiles = outcomeGatherer.GetAffectedTilesPerRadius(Center, Radius);
	}

	public bool TryGetExplosionOutcomeForCurrentRadius(out ReadOnlyHashSet<Vector3Int> readOnlyAffectedTiles)
	{
		if (_affectedTiles.TryGetValue(CurrentExplosionRadius, out var value))
		{
			readOnlyAffectedTiles = value.AsReadOnlyHashSet();
			return true;
		}
		return false;
	}

	public bool MoveToNextRadius()
	{
		return _affectedTiles.ContainsKey(CurrentExplosionRadius++);
	}
}

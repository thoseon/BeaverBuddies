using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.TerrainPhysics;

public interface ITerrainPhysicsService
{
	ReadOnlyList<Vector3Int> PhysicsSupportDeltas { get; }

	void GetTerrainAndBlockObjectStack(IEnumerable<BlockObject> inputBlockObjects, HashSet<Vector3Int> outputTerrain, HashSet<BlockObject> outputBlockObjects);

	void GetTerrainAndBlockObjectStack(IEnumerable<Vector3Int> inputTerrain, IEnumerable<BlockObject> inputBlockObjects, HashSet<Vector3Int> outputTerrain, HashSet<BlockObject> outputBlockObjects);

	void GetValidTerrainToAdd(ICollection<Vector3Int> inputTerrain, HashSet<Vector3Int> terrainToAdd);

	bool CanBeDestroyed(BlockObject blockObject);

	bool CanTerrainBeAdded(Vector3Int coordinates);

	bool ValidateBlockObjectPreview(BlockObject blockObject);
}

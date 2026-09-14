using UnityEngine;

namespace Timberborn.TerrainSystem;

public interface ITerrainRemovingEntity
{
	bool RemovesTerrainAt(Vector3Int coordinates);
}

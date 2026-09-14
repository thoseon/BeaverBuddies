using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.TerrainSystem;

public interface ICutoutTilesProvider
{
	IEnumerable<Vector3Int> GetPositionedCutoutTiles();
}

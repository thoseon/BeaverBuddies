using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.BlockSystem;

internal class BlockObjectTerrainCutout : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity, ICutoutTilesProvider
{
	private readonly TerrainCutout _terrainCutout;

	private BlockObject _blockObject;

	private BlockObjectTerrainCutoutSpec _blockObjectTerrainCutoutSpec;

	public BlockObjectTerrainCutout(TerrainCutout terrainCutout)
	{
		_terrainCutout = terrainCutout;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_blockObjectTerrainCutoutSpec = GetComponent<BlockObjectTerrainCutoutSpec>();
		Asserts.CollectionIsNotEmpty(_blockObjectTerrainCutoutSpec.CutoutTiles, "CutoutTiles");
	}

	public void InitializeEntity()
	{
		_terrainCutout.SetCutout(GetPositionedCutoutTiles());
	}

	public void DeleteEntity()
	{
		_terrainCutout.UnsetCutout(GetPositionedCutoutTiles());
	}

	public IEnumerable<Vector3Int> GetPositionedCutoutTiles()
	{
		return _blockObjectTerrainCutoutSpec.CutoutTiles.Select((Vector3Int tile) => _blockObject.TransformCoordinates(tile));
	}
}

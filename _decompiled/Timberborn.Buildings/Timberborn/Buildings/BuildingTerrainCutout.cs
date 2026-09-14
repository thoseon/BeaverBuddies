using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.Buildings;

internal class BuildingTerrainCutout : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity, ICutoutTilesProvider
{
	private readonly TerrainCutout _terrainCutout;

	private BlockObject _blockObject;

	private BlockObjectModelController _blockObjectModelController;

	private BuildingTerrainCutoutSpec _buildingTerrainCutoutSpec;

	private bool _isCutoutSet;

	public BuildingTerrainCutout(TerrainCutout terrainCutout)
	{
		_terrainCutout = terrainCutout;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_blockObjectModelController = GetComponent<BlockObjectModelController>();
		_buildingTerrainCutoutSpec = GetComponent<BuildingTerrainCutoutSpec>();
		Asserts.CollectionIsNotEmpty(_buildingTerrainCutoutSpec.CutoutTiles, "CutoutTiles");
	}

	public void InitializeEntity()
	{
		_blockObjectModelController.ModelsUpdated += OnModelsUpdated;
		UpdateCutout();
	}

	public void DeleteEntity()
	{
		_blockObjectModelController.ModelsUpdated -= OnModelsUpdated;
		_terrainCutout.UnsetCutout(GetPositionedCutoutTiles());
	}

	public IEnumerable<Vector3Int> GetPositionedCutoutTiles()
	{
		return _buildingTerrainCutoutSpec.CutoutTiles.Select((Vector3Int tile) => _blockObject.TransformCoordinates(tile));
	}

	private void OnModelsUpdated(object sender, EventArgs e)
	{
		UpdateCutout();
	}

	private void UpdateCutout()
	{
		if (!_blockObject.IsPreview)
		{
			if (_blockObjectModelController.IsFinishedModelShown && !_isCutoutSet)
			{
				_terrainCutout.SetCutout(GetPositionedCutoutTiles());
				_isCutoutSet = true;
			}
			else if (!_blockObjectModelController.IsFinishedModelShown && _isCutoutSet)
			{
				_terrainCutout.UnsetCutout(GetPositionedCutoutTiles());
				_isCutoutSet = false;
			}
		}
	}
}

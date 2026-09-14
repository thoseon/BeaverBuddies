using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.SoilBarrierSystem;

internal class SoilBarrier : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly SoilBarrierMap _soilBarrierMap;

	private readonly ITerrainService _terrainService;

	private BlockObject _blockObject;

	private SoilBarrierSpec _soilBarrierSpec;

	public SoilBarrier(SoilBarrierMap soilBarrierMap, ITerrainService terrainService)
	{
		_soilBarrierMap = soilBarrierMap;
		_terrainService = terrainService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_soilBarrierSpec = GetComponent<SoilBarrierSpec>();
	}

	public void OnEnterFinishedState()
	{
		foreach (Vector3Int groundCoordinate in GetGroundCoordinates())
		{
			if (_soilBarrierSpec.BlockAboveMoisture)
			{
				_soilBarrierMap.AddAboveMoistureBarrierAt(groundCoordinate);
			}
			if (_soilBarrierSpec.BlockFullMoisture)
			{
				_soilBarrierMap.AddFullMoistureBarrierAt(groundCoordinate);
			}
			if (_soilBarrierSpec.BlockContamination)
			{
				_soilBarrierMap.AddContaminationBarrierAt(groundCoordinate);
			}
		}
	}

	public void OnExitFinishedState()
	{
		foreach (Vector3Int groundCoordinate in GetGroundCoordinates())
		{
			if (_soilBarrierSpec.BlockAboveMoisture)
			{
				_soilBarrierMap.RemoveAboveMoistureBarrierAt(groundCoordinate);
			}
			if (_soilBarrierSpec.BlockFullMoisture)
			{
				_soilBarrierMap.RemoveFullMoistureBarrierAt(groundCoordinate);
			}
			if (_soilBarrierSpec.BlockContamination)
			{
				_soilBarrierMap.RemoveContaminationBarrierAt(groundCoordinate);
			}
		}
	}

	private IEnumerable<Vector3Int> GetGroundCoordinates()
	{
		foreach (Vector3Int foundationCoordinate in _blockObject.PositionedBlocks.GetFoundationCoordinates())
		{
			if (_terrainService.OnGround(foundationCoordinate))
			{
				yield return foundationCoordinate;
			}
		}
	}
}

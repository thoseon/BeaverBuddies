using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bindito.Unity;
using Timberborn.AssetSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.TerrainSystem;
using Timberborn.Workshops;
using UnityEngine;

namespace Timberborn.Terraforming;

internal class Drill : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly ITerrainService _terrainService;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly IInstantiator _instantiator;

	private readonly IAssetLoader _assetLoader;

	private BlockObject _blockObject;

	private Manufactory _manufactory;

	private DrillSpec _drillSpec;

	private GameObject _removalEffectPrefab;

	private ImmutableArray<Vector3Int> _positionedDrillableCoordinates;

	private readonly List<Vector3Int> _drillCandidates = new List<Vector3Int>();

	public int DrillingLevel { get; private set; }

	public Drill(ITerrainService terrainService, IRandomNumberGenerator randomNumberGenerator, IInstantiator instantiator, IAssetLoader assetLoader)
	{
		_terrainService = terrainService;
		_randomNumberGenerator = randomNumberGenerator;
		_instantiator = instantiator;
		_assetLoader = assetLoader;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_manufactory = GetComponent<Manufactory>();
		_drillSpec = GetComponent<DrillSpec>();
		_removalEffectPrefab = _assetLoader.Load<GameObject>(_drillSpec.RemovalEffectPath);
	}

	public void OnEnterFinishedState()
	{
		PositionDrillableCoordinates();
		SetInitialDrillingLevel();
		_manufactory.ProductionFinished += OnProductionFinished;
	}

	public void OnExitFinishedState()
	{
		_manufactory.ProductionFinished -= OnProductionFinished;
	}

	private void PositionDrillableCoordinates()
	{
		ImmutableArray<Vector3Int> drillableCoordinates = _drillSpec.DrillableCoordinates;
		_positionedDrillableCoordinates = drillableCoordinates.Select((Vector3Int coordinates) => _blockObject.TransformCoordinates(coordinates)).ToImmutableArray();
	}

	private void SetInitialDrillingLevel()
	{
		DrillingLevel = _positionedDrillableCoordinates.Select((Vector3Int coordinates) => _terrainService.GetTerrainHeightBelow(coordinates)).Max() - 1;
	}

	private void OnProductionFinished(object sender, EventArgs e)
	{
		TryDrillTerrain();
	}

	private void TryDrillTerrain()
	{
		bool flag = false;
		while (DrillingLevel >= 0 && !flag)
		{
			CollectDrillCandidates();
			flag = TryDrillRandomTerrainBlock();
			TryLowerDrillLevel();
		}
		_drillCandidates.Clear();
	}

	private void CollectDrillCandidates()
	{
		foreach (Vector3Int positionedDrillableCoordinate in _positionedDrillableCoordinates)
		{
			if (_terrainService.GetTerrainHeightBelow(positionedDrillableCoordinate) > DrillingLevel)
			{
				_drillCandidates.Add(new Vector3Int(positionedDrillableCoordinate.x, positionedDrillableCoordinate.y, DrillingLevel));
			}
		}
	}

	private bool TryDrillRandomTerrainBlock()
	{
		if (_drillCandidates.Count > 0)
		{
			Vector3Int listElement = _randomNumberGenerator.GetListElement(_drillCandidates);
			_terrainService.UnsetTerrain(listElement);
			SpawnDrillEffect(listElement);
			_drillCandidates.Remove(listElement);
			return true;
		}
		return false;
	}

	private void TryLowerDrillLevel()
	{
		if (_drillCandidates.Count == 0)
		{
			DrillingLevel--;
		}
	}

	private void SpawnDrillEffect(Vector3Int effectLocation)
	{
		_instantiator.Instantiate(_removalEffectPrefab, base.Transform).transform.position = CoordinateSystem.GridToWorldCentered(effectLocation);
	}
}

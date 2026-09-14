using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.TerrainSystem;

namespace Timberborn.PathSystem;

internal class DrivewayModels : BaseComponent, IAwakableComponent, IModelUpdater
{
	private readonly IConnectionService _connectionService;

	private readonly ITerrainService _terrainService;

	private readonly DrivewayModelInstantiator _drivewayModelInstantiator;

	private readonly IBlockService _blockService;

	private readonly List<DrivewayModel> _drivewayModels = new List<DrivewayModel>();

	private BlockObject _blockObject;

	internal DrivewayModels(IConnectionService connectionService, ITerrainService terrainService, DrivewayModelInstantiator drivewayModelInstantiator, IBlockService blockService)
	{
		_connectionService = connectionService;
		_terrainService = terrainService;
		_drivewayModelInstantiator = drivewayModelInstantiator;
		_blockService = blockService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		foreach (DrivewayModelSpec driveway in GetComponent<DrivewayModelsSpec>().Driveways)
		{
			_drivewayModels.Add(CreateDrivewayModel(driveway));
		}
	}

	public void UpdateModel()
	{
		foreach (DrivewayModel drivewayModel in _drivewayModels)
		{
			drivewayModel.UpdateModel();
		}
	}

	private DrivewayModel CreateDrivewayModel(DrivewayModelSpec spec)
	{
		DrivewayModel drivewayModel = new DrivewayModel(_connectionService, _terrainService, _drivewayModelInstantiator, _blockService, _blockObject, spec);
		drivewayModel.ValidateDriveway();
		return drivewayModel;
	}
}

using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.TerrainPhysics;

namespace Timberborn.ConstructionSites;

public class PhysicallySupportedConstructionSite : BaseComponent, IAwakableComponent, IUnfinishedStateListener, IConstructionSiteValidator
{
	private readonly ITerrainPhysicsService _terrainPhysicsService;

	private BlockObject _blockObject;

	public bool IsValid { get; private set; } = true;

	public bool IsModelValid => IsValid;

	public event EventHandler ValidationStateChanged;

	public PhysicallySupportedConstructionSite(ITerrainPhysicsService terrainPhysicsService)
	{
		_terrainPhysicsService = terrainPhysicsService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void OnEnterUnfinishedState()
	{
		Validate();
	}

	public void OnExitUnfinishedState()
	{
	}

	public void Validate()
	{
		bool isValid = IsValid;
		IsValid = _terrainPhysicsService.CanTerrainBeAdded(_blockObject.Coordinates);
		if (isValid != IsValid)
		{
			ValidationStateChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}

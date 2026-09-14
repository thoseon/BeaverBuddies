using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.ConstructionSites;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.Terraforming;

internal class GroundRaisingService : ILoadableSingleton
{
	private readonly ITerrainService _terrainService;

	private readonly IBlockService _blockService;

	private readonly EventBus _eventBus;

	public GroundRaisingService(ITerrainService terrainService, IBlockService blockService, EventBus eventBus)
	{
		_terrainService = terrainService;
		_blockService = blockService;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_terrainService.TerrainHeightChanged += OnTerrainHeightChanged;
	}

	[OnEvent]
	public void OnEntityDeleted(EntityDeletedEvent entityDeletedEvent)
	{
		GroundRaiser component = entityDeletedEvent.Entity.GetComponent<GroundRaiser>();
		if ((bool)component && component.ShouldRaiseTerrain)
		{
			_terrainService.SetTerrain(component.Coordinates);
			component.GetComponent<GroundedConstructionSite>().UpdateConstructionSitesAtop();
			component.GetComponent<PhysicallySupportedConstructionSiteUpdater>().UpdateNeighbours();
		}
	}

	private void OnTerrainHeightChanged(object sender, TerrainHeightChangeEventArgs terrainHeightChangeEventArgs)
	{
		TerrainHeightChange change = terrainHeightChangeEventArgs.Change;
		if (change.SetTerrain)
		{
			return;
		}
		for (int i = change.From; i <= change.To; i++)
		{
			Vector3Int vector3Int = change.Coordinates.ToVector3Int(i);
			Vector3Int[] neighbors4Vector3Int = Deltas.Neighbors4Vector3Int;
			foreach (Vector3Int vector3Int2 in neighbors4Vector3Int)
			{
				ValidateConstructionSite(vector3Int + vector3Int2);
			}
		}
		ValidateConstructionSite(change.Coordinates.ToVector3Int(change.To + 1));
	}

	private void ValidateConstructionSite(Vector3Int coordinates)
	{
		PhysicallySupportedConstructionSite bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<PhysicallySupportedConstructionSite>(coordinates);
		if ((bool)bottomObjectComponentAt)
		{
			bottomObjectComponentAt.Validate();
		}
	}
}

using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.TerrainPhysics;
using Timberborn.WorldPersistence;

namespace Timberborn.BlockAndTerrainLoadValidation;

internal class BlockAndTerrainBatchLoader : IEntityBatchLoader
{
	private readonly BlockObjectBatchLoader _blockObjectBatchLoader;

	private readonly TerrainPhysicsPostLoader _terrainPhysicsPostLoader;

	public BlockAndTerrainBatchLoader(BlockObjectBatchLoader blockObjectBatchLoader, TerrainPhysicsPostLoader terrainPhysicsPostLoader)
	{
		_blockObjectBatchLoader = blockObjectBatchLoader;
		_terrainPhysicsPostLoader = terrainPhysicsPostLoader;
	}

	public void BatchLoadEntities(IEnumerable<EntityComponent> entities)
	{
		_blockObjectBatchLoader.AddToServices(entities);
		_terrainPhysicsPostLoader.ValidateAll();
	}
}

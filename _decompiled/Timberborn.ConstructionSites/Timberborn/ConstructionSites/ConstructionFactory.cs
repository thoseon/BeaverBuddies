using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;

namespace Timberborn.ConstructionSites;

public class ConstructionFactory
{
	private readonly BlockObjectFactory _blockObjectFactory;

	public ConstructionFactory(BlockObjectFactory blockObjectFactory)
	{
		_blockObjectFactory = blockObjectFactory;
	}

	public BlockObject CreateAsUnfinished(EntitySetup.Builder entitySetupBuilder, Placement placement)
	{
		return _blockObjectFactory.CreateUnfinished(entitySetupBuilder, placement);
	}

	public BlockObject CreateAsFinished(EntitySetup.Builder entitySetupBuilder, Placement placement)
	{
		entitySetupBuilder.AddInitComponent(new FinishedConstructionSiteInit());
		return CreateAsUnfinished(entitySetupBuilder, placement);
	}
}

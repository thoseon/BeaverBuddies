using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.ToolSystemUI;

namespace Timberborn.BlockObjectTools;

public class DefaultBlockObjectPlacer : IBlockObjectPlacer
{
	private readonly BlockObjectFactory _blockObjectFactory;

	public DefaultBlockObjectPlacer(BlockObjectFactory blockObjectFactory)
	{
		_blockObjectFactory = blockObjectFactory;
	}

	public void Place(EntitySetup.Builder entitySetupBuilder, Placement placement)
	{
		_blockObjectFactory.CreateFinished(entitySetupBuilder, placement);
	}

	public void Describe(BlockObjectTool tool, ToolDescription.Builder builder, Preview preview)
	{
	}

	public bool CanHandle(BlockObjectSpec template)
	{
		return true;
	}
}

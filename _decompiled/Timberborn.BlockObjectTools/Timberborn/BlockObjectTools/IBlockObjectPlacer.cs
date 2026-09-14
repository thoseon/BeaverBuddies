using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.ToolSystemUI;

namespace Timberborn.BlockObjectTools;

public interface IBlockObjectPlacer
{
	void Place(EntitySetup.Builder entitySetupBuilder, Placement placement);

	void Describe(BlockObjectTool tool, ToolDescription.Builder builder, Preview preview);

	bool CanHandle(BlockObjectSpec template);
}

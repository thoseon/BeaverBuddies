using Timberborn.ToolSystemUI;

namespace Timberborn.BlockObjectTools;

public interface IBlockObjectToolDescriber
{
	ToolDescription Describe(BlockObjectTool blockObjectTool, IBlockObjectPlacer blockObjectPlacer);
}

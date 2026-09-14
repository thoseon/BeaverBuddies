using Timberborn.BlockObjectTools;
using Timberborn.BlockSystem;

namespace Timberborn.GameStartup;

internal class StartingBuildingToolFactory
{
	private readonly BlockObjectToolFactory _blockObjectToolFactory;

	private readonly StartingBuildingToolDescriber _startingBuildingToolDescriber;

	private readonly StartingBuildingPlacer _startingBuildingPlacer;

	public StartingBuildingToolFactory(BlockObjectToolFactory blockObjectToolFactory, StartingBuildingToolDescriber startingBuildingToolDescriber, StartingBuildingPlacer startingBuildingPlacer)
	{
		_blockObjectToolFactory = blockObjectToolFactory;
		_startingBuildingToolDescriber = startingBuildingToolDescriber;
		_startingBuildingPlacer = startingBuildingPlacer;
	}

	public BlockObjectTool Create(PlaceableBlockObjectSpec template)
	{
		return _blockObjectToolFactory.Create(template, _startingBuildingPlacer, _startingBuildingToolDescriber);
	}
}

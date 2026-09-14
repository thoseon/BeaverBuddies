using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BlockObjectTools;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.BottomBarSystem;
using Timberborn.ConstructionMode;
using Timberborn.ToolButtonSystem;
using Timberborn.ToolSystem;

namespace Timberborn.BlockObjectToolsUI;

public class BlockObjectToolGroupButtonFactory
{
	private readonly BlockObjectToolButtonFactory _blockObjectToolButtonFactory;

	private readonly ToolGroupButtonFactory _toolGroupButtonFactory;

	private readonly ToolGroupService _toolGroupService;

	public BlockObjectToolGroupButtonFactory(BlockObjectToolButtonFactory blockObjectToolButtonFactory, ToolGroupButtonFactory toolGroupButtonFactory, ToolGroupService toolGroupService)
	{
		_blockObjectToolButtonFactory = blockObjectToolButtonFactory;
		_toolGroupButtonFactory = toolGroupButtonFactory;
		_toolGroupService = toolGroupService;
	}

	public BottomBarElement Create(BlockObjectToolGroupSpec blockObjectToolGroupSpec, IEnumerable<PlaceableBlockObjectSpec> blockObjects)
	{
		ToolGroupSpec spec = CreateBlueprint(blockObjectToolGroupSpec).GetSpec<ToolGroupSpec>();
		ToolGroupButton toolGroupButton = _toolGroupButtonFactory.CreateGreen(spec);
		foreach (PlaceableBlockObjectSpec blockObject in blockObjects)
		{
			if (blockObject.UsableWithCurrentFeatureToggles)
			{
				ToolButton toolButton = _blockObjectToolButtonFactory.Create(blockObject, toolGroupButton.ToolButtonsElement);
				_toolGroupService.AssignToGroup(spec, toolButton.Tool);
				toolGroupButton.AddTool(toolButton);
			}
		}
		_toolGroupService.RegisterGroup(spec);
		return BottomBarElement.CreateMultiLevel(toolGroupButton.Root, toolGroupButton.ToolButtonsElement);
	}

	private static Blueprint CreateBlueprint(BlockObjectToolGroupSpec blockObjectToolGroupSpec)
	{
		ToolGroupSpec toolGroupSpec = new ToolGroupSpec
		{
			Id = "BlockObjectToolGroupSpec." + blockObjectToolGroupSpec.Id,
			DisplayNameLocKey = blockObjectToolGroupSpec.NameLocKey,
			Icon = blockObjectToolGroupSpec.Icon
		};
		return new Blueprint("Blueprint." + blockObjectToolGroupSpec.Id, new ComponentSpec[2]
		{
			toolGroupSpec,
			new ConstructionModeToolGroupSpec()
		}, ImmutableArray<Blueprint>.Empty);
	}
}

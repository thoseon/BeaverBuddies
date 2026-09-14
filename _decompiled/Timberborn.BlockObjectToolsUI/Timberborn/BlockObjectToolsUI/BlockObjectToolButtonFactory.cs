using System;
using Timberborn.BlockObjectTools;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.ToolButtonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.BlockObjectToolsUI;

public class BlockObjectToolButtonFactory
{
	private readonly ToolButtonFactory _toolButtonFactory;

	private readonly BlockObjectToolFactory _blockObjectToolFactory;

	private readonly BlockObjectToolDescriber _blockObjectToolDescriber;

	private readonly BlockObjectPlacerService _blockObjectPlacerService;

	public BlockObjectToolButtonFactory(ToolButtonFactory toolButtonFactory, BlockObjectToolFactory blockObjectToolFactory, BlockObjectToolDescriber blockObjectToolDescriber, BlockObjectPlacerService blockObjectPlacerService)
	{
		_toolButtonFactory = toolButtonFactory;
		_blockObjectToolFactory = blockObjectToolFactory;
		_blockObjectToolDescriber = blockObjectToolDescriber;
		_blockObjectPlacerService = blockObjectPlacerService;
	}

	public ToolButton Create(PlaceableBlockObjectSpec template, VisualElement buttonParent)
	{
		BlockObjectTool tool = CreateTool(template);
		Sprite toolImage = GetToolImage(template);
		return template.ToolShape switch
		{
			ToolShapes.Square => _toolButtonFactory.Create(tool, toolImage, buttonParent), 
			ToolShapes.Hex => _toolButtonFactory.CreateHex(tool, toolImage, buttonParent), 
			_ => throw new ArgumentOutOfRangeException($"Invalid tool shape: {template.ToolShape}"), 
		};
	}

	public ToolButton Create(PlaceableBlockObjectSpec template)
	{
		return _toolButtonFactory.CreateGrouplessGreen(CreateTool(template), GetToolImage(template));
	}

	private BlockObjectTool CreateTool(PlaceableBlockObjectSpec template)
	{
		BlockObjectSpec spec = template.GetSpec<BlockObjectSpec>();
		IBlockObjectPlacer matchingPlacer = _blockObjectPlacerService.GetMatchingPlacer(spec);
		return _blockObjectToolFactory.Create(template, matchingPlacer, _blockObjectToolDescriber);
	}

	private static Sprite GetToolImage(PlaceableBlockObjectSpec template)
	{
		return template.GetSpec<LabeledEntitySpec>().Icon.Asset;
	}
}

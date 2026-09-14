using System.Collections.Generic;
using Timberborn.BlockObjectTools;
using Timberborn.BlockObjectToolsUI;
using Timberborn.BlockSystem;
using Timberborn.BottomBarSystem;
using Timberborn.ToolButtonSystem;

namespace Timberborn.MapEditorUI;

internal class MapEditorBlockObjectButtons : IBottomBarElementsProvider
{
	private static readonly string SingleLevelGroup = "MapEditor";

	private static readonly string[] NestedGroups = new string[3] { "MapEditorWater", "MapEditorObjects", "Ruins" };

	private readonly BlockObjectToolGroupSpecService _blockObjectToolGroupSpecService;

	private readonly PlaceableBlockObjectSpecService _placeableBlockObjectSpecService;

	private readonly BlockObjectToolGroupButtonFactory _blockObjectToolGroupButtonFactory;

	private readonly BlockObjectToolButtonFactory _blockObjectToolButtonFactory;

	public MapEditorBlockObjectButtons(BlockObjectToolGroupSpecService blockObjectToolGroupSpecService, PlaceableBlockObjectSpecService placeableBlockObjectSpecService, BlockObjectToolGroupButtonFactory blockObjectToolGroupButtonFactory, BlockObjectToolButtonFactory blockObjectToolButtonFactory)
	{
		_blockObjectToolGroupSpecService = blockObjectToolGroupSpecService;
		_placeableBlockObjectSpecService = placeableBlockObjectSpecService;
		_blockObjectToolGroupButtonFactory = blockObjectToolGroupButtonFactory;
		_blockObjectToolButtonFactory = blockObjectToolButtonFactory;
	}

	public IEnumerable<BottomBarElement> GetElements()
	{
		BlockObjectToolGroupSpec spec = _blockObjectToolGroupSpecService.GetSpec(SingleLevelGroup);
		IEnumerable<PlaceableBlockObjectSpec> blockObjects = _placeableBlockObjectSpecService.GetBlockObjects(spec);
		foreach (PlaceableBlockObjectSpec item in blockObjects)
		{
			ToolButton toolButton = _blockObjectToolButtonFactory.Create(item);
			yield return BottomBarElement.CreateSingleLevel(toolButton.Root);
		}
		string[] nestedGroups = NestedGroups;
		foreach (string groupId in nestedGroups)
		{
			BlockObjectToolGroupSpec spec2 = _blockObjectToolGroupSpecService.GetSpec(groupId);
			IEnumerable<PlaceableBlockObjectSpec> blockObjects2 = _placeableBlockObjectSpecService.GetBlockObjects(spec2);
			yield return _blockObjectToolGroupButtonFactory.Create(spec2, blockObjects2);
		}
	}
}

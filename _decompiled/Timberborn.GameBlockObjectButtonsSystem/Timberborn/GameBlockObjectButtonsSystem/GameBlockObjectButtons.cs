using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockObjectTools;
using Timberborn.BlockObjectToolsUI;
using Timberborn.BlockSystem;
using Timberborn.BottomBarSystem;
using Timberborn.Common;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace Timberborn.GameBlockObjectButtonsSystem;

public class GameBlockObjectButtons : IBottomBarElementsProvider
{
	private readonly BlockObjectToolGroupSpecService _blockObjectToolGroupSpecService;

	private readonly PlaceableBlockObjectSpecService _placeableBlockObjectSpecService;

	private readonly BlockObjectToolGroupButtonFactory _blockObjectToolGroupButtonFactory;

	public GameBlockObjectButtons(BlockObjectToolGroupSpecService blockObjectToolGroupSpecService, PlaceableBlockObjectSpecService placeableBlockObjectSpecService, BlockObjectToolGroupButtonFactory blockObjectToolGroupButtonFactory)
	{
		_blockObjectToolGroupSpecService = blockObjectToolGroupSpecService;
		_placeableBlockObjectSpecService = placeableBlockObjectSpecService;
		_blockObjectToolGroupButtonFactory = blockObjectToolGroupButtonFactory;
	}

	public IEnumerable<BottomBarElement> GetElements()
	{
		return CreateRegularBlockObjectToolGroups().Concat(CreateFallbackBlockObjectToolGroup());
	}

	private IEnumerable<BottomBarElement> CreateRegularBlockObjectToolGroups()
	{
		IEnumerable<BlockObjectToolGroupSpec> enumerable = _blockObjectToolGroupSpecService.AllSpecs.Where((BlockObjectToolGroupSpec toolGroupSpec) => !toolGroupSpec.FallbackGroup);
		foreach (BlockObjectToolGroupSpec item in enumerable)
		{
			BottomBarElement? bottomBarElement = CreateRegularBlockObjectToolGroup(item);
			if (bottomBarElement.HasValue)
			{
				yield return bottomBarElement.Value;
			}
		}
	}

	private BottomBarElement? CreateRegularBlockObjectToolGroup(BlockObjectToolGroupSpec blockObjectToolGroupSpec)
	{
		List<PlaceableBlockObjectSpec> list = _placeableBlockObjectSpecService.GetBlockObjects(blockObjectToolGroupSpec).ToList();
		if (list.Count == 0)
		{
			return null;
		}
		return _blockObjectToolGroupButtonFactory.Create(blockObjectToolGroupSpec, list);
	}

	private IEnumerable<BottomBarElement> CreateFallbackBlockObjectToolGroup()
	{
		List<PlaceableBlockObjectSpec> list = _placeableBlockObjectSpecService.GetBlockObjectsWithoutValidGroup().ToList();
		if (!list.IsEmpty())
		{
			LogBlockObjectsWithUnknownToolGroup(list);
			BlockObjectToolGroupSpec fallbackSpec = _blockObjectToolGroupSpecService.GetFallbackSpec();
			yield return _blockObjectToolGroupButtonFactory.Create(fallbackSpec, list);
		}
	}

	private static void LogBlockObjectsWithUnknownToolGroup(IEnumerable<PlaceableBlockObjectSpec> blockObjects)
	{
		foreach (PlaceableBlockObjectSpec blockObject in blockObjects)
		{
			string templateName = blockObject.GetSpec<TemplateSpec>().TemplateName;
			string toolGroupId = blockObject.ToolGroupId;
			Debug.LogWarning("Block object \"" + templateName + "\" is associated with an unknown BlockObjectToolGroupSpec with ID \"" + toolGroupId + "\"");
		}
	}
}

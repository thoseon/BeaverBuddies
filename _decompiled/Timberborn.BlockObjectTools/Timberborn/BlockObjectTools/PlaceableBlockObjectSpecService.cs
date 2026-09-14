using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.TemplateSystem;

namespace Timberborn.BlockObjectTools;

public class PlaceableBlockObjectSpecService
{
	private readonly BlockObjectToolGroupSpecService _blockObjectToolGroupSpecService;

	private readonly TemplateService _templateService;

	public PlaceableBlockObjectSpecService(BlockObjectToolGroupSpecService blockObjectToolGroupSpecService, TemplateService templateService)
	{
		_blockObjectToolGroupSpecService = blockObjectToolGroupSpecService;
		_templateService = templateService;
	}

	public IEnumerable<PlaceableBlockObjectSpec> GetBlockObjects(BlockObjectToolGroupSpec blockObjectToolGroupSpec)
	{
		return from spec in _templateService.GetAll<PlaceableBlockObjectSpec>()
			where blockObjectToolGroupSpec.Id == spec.ToolGroupId
			orderby spec.ToolOrder, spec.GetSpec<LabeledEntitySpec>().DisplayNameLocKey
			select spec;
	}

	public IEnumerable<PlaceableBlockObjectSpec> GetBlockObjectsWithoutValidGroup()
	{
		HashSet<string> toolGroupIds = _blockObjectToolGroupSpecService.AllSpecs.Select((BlockObjectToolGroupSpec toolGroupSpec) => toolGroupSpec.Id).ToHashSet();
		return from spec in _templateService.GetAll<PlaceableBlockObjectSpec>()
			where !toolGroupIds.Contains(spec.ToolGroupId)
			orderby spec.ToolOrder, spec.GetSpec<LabeledEntitySpec>().DisplayNameLocKey
			select spec;
	}
}

using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.BlockObjectTools;

public class BlockObjectToolGroupSpecService : ILoadableSingleton
{
	private readonly ISpecService _specService;

	public ImmutableArray<BlockObjectToolGroupSpec> AllSpecs { get; private set; }

	public BlockObjectToolGroupSpecService(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		AllSpecs = (from spec in _specService.GetSpecs<BlockObjectToolGroupSpec>()
			orderby spec.Order
			select spec).ToImmutableArray();
	}

	public BlockObjectToolGroupSpec GetFallbackSpec()
	{
		return AllSpecs.Single((BlockObjectToolGroupSpec spec) => spec.FallbackGroup);
	}

	public BlockObjectToolGroupSpec GetSpec(string groupId)
	{
		return AllSpecs.Single((BlockObjectToolGroupSpec spec) => spec.Id == groupId);
	}
}

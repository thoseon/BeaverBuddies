using System.Collections.Generic;
using System.Linq;
using Timberborn.BlockSystem;

namespace Timberborn.BlockObjectTools;

public class BlockObjectPlacerService
{
	private readonly DefaultBlockObjectPlacer _defaultBlockObjectPlacer;

	private readonly List<IBlockObjectPlacer> _blockObjectPlacers;

	public BlockObjectPlacerService(DefaultBlockObjectPlacer defaultBlockObjectPlacer, IEnumerable<IBlockObjectPlacer> blockObjectPlacers)
	{
		_defaultBlockObjectPlacer = defaultBlockObjectPlacer;
		_blockObjectPlacers = blockObjectPlacers.ToList();
	}

	public IBlockObjectPlacer GetMatchingPlacer(BlockObjectSpec spec)
	{
		IBlockObjectPlacer blockObjectPlacer = _blockObjectPlacers.SingleOrDefault((IBlockObjectPlacer placer) => placer.CanHandle(spec));
		return blockObjectPlacer ?? _defaultBlockObjectPlacer;
	}
}

using Timberborn.BlockSystem;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.BlockSystemNavigation;

internal class BlockSystemNavMeshSizeProvider : INavMeshSizeProvider
{
	private readonly IBlockService _blockService;

	public Vector3Int Size => _blockService.Size;

	public BlockSystemNavMeshSizeProvider(IBlockService blockService)
	{
		_blockService = blockService;
	}
}

using Timberborn.BlockSystem;
using UnityEngine;

namespace Timberborn.PathSystem;

internal class PathService : IPathService
{
	private readonly IBlockService _blockService;

	private readonly PreviewBlockService _previewBlockService;

	public PathService(IBlockService blockService, PreviewBlockService previewBlockService)
	{
		_blockService = blockService;
		_previewBlockService = previewBlockService;
	}

	public bool IsPath(Vector3Int coordinates)
	{
		return IsPath(coordinates, allowUnfinished: true);
	}

	private bool IsPath(Vector3Int coordinates, bool allowUnfinished)
	{
		if (!allowUnfinished)
		{
			return IsPath(_blockService.GetPathObjectAt(coordinates), allowUnfinished: false);
		}
		if (!IsPath(_blockService.GetPathObjectAt(coordinates), allowUnfinished: true))
		{
			return IsPath(_previewBlockService.GetPathObjectAt(coordinates), allowUnfinished: true);
		}
		return true;
	}

	private static bool IsPath(BlockObject blockObject, bool allowUnfinished)
	{
		if ((bool)blockObject && (allowUnfinished || blockObject.IsFinished))
		{
			return blockObject.HasComponent<PathSpec>();
		}
		return false;
	}
}

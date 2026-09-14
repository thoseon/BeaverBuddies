using System;
using System.Collections.Immutable;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.ZiplineSystem;

public class ZiplineConnectionBlockFactory : ILoadableSingleton
{
	private static readonly string ZiplineConnectionBlockPath = "Models/ZiplineCable/ZiplineConnectionBlock.blueprint";

	private readonly BlockObjectFactory _blockObjectFactory;

	private readonly ISpecService _specService;

	public BlockObjectSpec ZiplineConnectionBlock { get; private set; }

	public BlockOccupations ZiplineConnectionOccupation { get; private set; }

	public ZiplineConnectionBlockFactory(BlockObjectFactory blockObjectFactory, ISpecService specService)
	{
		_blockObjectFactory = blockObjectFactory;
		_specService = specService;
	}

	public void Load()
	{
		ZiplineConnectionBlock = _specService.GetBlueprint(ZiplineConnectionBlockPath).GetSpec<BlockObjectSpec>();
		ImmutableArray<BlockSpec> blocks = ZiplineConnectionBlock.Blocks;
		if (blocks.Length != 1)
		{
			throw new InvalidOperationException("Zipline connection block must be 1x1x1 in size");
		}
		ZiplineConnectionOccupation = blocks[0].Occupations;
	}

	public BlockObject CreateConnection(Transform parent, Vector3Int gridPosition)
	{
		BlockObject blockObject = _blockObjectFactory.CreateAsPreview(ZiplineConnectionBlock, parent, new Placement(gridPosition));
		blockObject.GameObject.name = string.Format("{0} {1}", "ZiplineConnectionBlockPath", gridPosition);
		blockObject.MarkAsFinishedAndAddToServices();
		return blockObject;
	}
}

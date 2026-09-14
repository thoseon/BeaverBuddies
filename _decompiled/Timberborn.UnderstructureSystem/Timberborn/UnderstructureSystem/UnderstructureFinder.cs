using Timberborn.BlockSystem;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace Timberborn.UnderstructureSystem;

internal class UnderstructureFinder
{
	private readonly IBlockService _blockService;

	public UnderstructureFinder(IBlockService blockService)
	{
		_blockService = blockService;
	}

	public BlockObject FindNonStrict(BlockObject blockObject, UnderstructureConstraint understructureConstraint)
	{
		foreach (string understructureTemplateName in understructureConstraint.UnderstructureTemplateNames)
		{
			foreach (Block foundationBlock in blockObject.PositionedBlocks.GetFoundationBlocks())
			{
				if (BlockShouldBeOnUnderstructure(foundationBlock))
				{
					BlockObject underlyingObject = GetUnderlyingObject(foundationBlock.Coordinates, understructureTemplateName);
					if ((bool)underlyingObject)
					{
						return underlyingObject;
					}
				}
			}
		}
		return null;
	}

	public BlockObject FindStrict(BlockObject blockObject, UnderstructureConstraint understructureConstraint)
	{
		foreach (string understructureTemplateName in understructureConstraint.UnderstructureTemplateNames)
		{
			BlockObject blockObject2 = FindStrict(blockObject, understructureTemplateName);
			if (blockObject2 != null)
			{
				return blockObject2;
			}
		}
		return null;
	}

	private BlockObject FindStrict(BlockObject blockObject, string templateName)
	{
		BlockObject blockObject2 = null;
		foreach (Block foundationBlock in blockObject.PositionedBlocks.GetFoundationBlocks())
		{
			if (BlockShouldBeOnUnderstructure(foundationBlock))
			{
				BlockObject underlyingObject = GetUnderlyingObject(foundationBlock.Coordinates, templateName);
				if (underlyingObject == null)
				{
					return null;
				}
				if (blockObject2 == null)
				{
					blockObject2 = underlyingObject;
				}
				else if (blockObject2 != underlyingObject)
				{
					return null;
				}
			}
		}
		return blockObject2;
	}

	private BlockObject GetUnderlyingObject(Vector3Int coordinates, string requiredTemplateName)
	{
		foreach (BlockObject item in _blockService.GetObjectsAt(coordinates))
		{
			TemplateSpec component = item.GetComponent<TemplateSpec>();
			if ((object)component != null && component.IsNamed(requiredTemplateName))
			{
				return item;
			}
		}
		return null;
	}

	private static bool BlockShouldBeOnUnderstructure(Block foundationBlock)
	{
		return foundationBlock.Occupation.IsTopOrCornersOrBoth();
	}
}

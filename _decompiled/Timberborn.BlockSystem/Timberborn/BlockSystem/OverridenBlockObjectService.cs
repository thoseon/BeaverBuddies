using System.Collections.Generic;
using Microsoft.Extensions.ObjectPool;
using Timberborn.EntitySystem;

namespace Timberborn.BlockSystem;

public class OverridenBlockObjectService
{
	private class ListPolicy<T> : PooledObjectPolicy<List<T>>
	{
		public override List<T> Create()
		{
			return new List<T>();
		}

		public override bool Return(List<T> target)
		{
			target.Clear();
			return true;
		}
	}

	private readonly IBlockService _blockService;

	private readonly EntityService _entityService;

	private readonly List<BlockObject> _blockObjectCache = new List<BlockObject>();

	private readonly ObjectPool<List<BlockObject>> _intersectionPool = new DefaultObjectPool<List<BlockObject>>(new ListPolicy<BlockObject>());

	public OverridenBlockObjectService(IBlockService blockService, EntityService entityService)
	{
		_blockService = blockService;
		_entityService = entityService;
	}

	public void OverrideBlockObjects(BlockObject blockObject)
	{
		List<BlockObject> list = _intersectionPool.Get();
		foreach (Block allBlock in blockObject.PositionedBlocks.GetAllBlocks())
		{
			_blockService.GetIntersectingObjectsAt(allBlock.Coordinates, allBlock.Occupation, _blockObjectCache);
			foreach (BlockObject item in _blockObjectCache)
			{
				if (item.Overridable)
				{
					list.Add(item);
				}
			}
			_blockObjectCache.Clear();
		}
		foreach (BlockObject item2 in list)
		{
			ProcessIntersectingBlockObject(blockObject, item2);
		}
		_intersectionPool.Return(list);
	}

	private void ProcessIntersectingBlockObject(BlockObject blockObject, BlockObject intersection)
	{
		IBlockObjectCustomOverriding component = intersection.GetComponent<IBlockObjectCustomOverriding>();
		if (component != null)
		{
			component.OverrideBy(blockObject);
		}
		else
		{
			_entityService.Delete(intersection);
		}
	}
}

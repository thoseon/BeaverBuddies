using System;

namespace Timberborn.TickSystem;

internal class TickableBucketService : ITickableBucketService
{
	private static readonly int NumberOfEntityBuckets = 128;

	private static readonly int NumberOfSingletonBuckets = 1;

	private readonly ITickableSingletonService _tickableSingletonService;

	private readonly TickableEntityBucket[] _tickableEntityBuckets;

	private int _nextBucketIndex;

	public int TotalNumberOfBuckets => NumberOfEntityBuckets + NumberOfSingletonBuckets;

	public TickableBucketService(ITickableSingletonService tickableSingletonService)
	{
		_tickableSingletonService = tickableSingletonService;
		_tickableEntityBuckets = new TickableEntityBucket[NumberOfEntityBuckets];
		for (int i = 0; i < _tickableEntityBuckets.Length; i++)
		{
			_tickableEntityBuckets[i] = new TickableEntityBucket();
		}
	}

	public void AddEntity(TickableEntity tickableEntity)
	{
		GetEntityBucket(tickableEntity).Add(tickableEntity);
	}

	public void RemoveEntity(TickableEntity tickableEntity)
	{
		GetEntityBucket(tickableEntity).Remove(tickableEntity);
	}

	public void TickBuckets(int numberOfBucketsToTick)
	{
		while (numberOfBucketsToTick-- > 0)
		{
			TickNextBucket();
		}
	}

	public void FinishFullTick()
	{
		while (_nextBucketIndex != 0)
		{
			TickNextBucket();
		}
		_tickableSingletonService.ForceFinishParallelTick();
	}

	public void TickOnce()
	{
		if (_nextBucketIndex != 0)
		{
			FinishFullTick();
		}
		TickNextBucket();
		FinishFullTick();
	}

	private TickableEntityBucket GetEntityBucket(TickableEntity tickableEntity)
	{
		int entityBucketIndex = GetEntityBucketIndex(tickableEntity.EntityId);
		return _tickableEntityBuckets[entityBucketIndex];
	}

	private static int GetEntityBucketIndex(Guid entityId)
	{
		return (int)((uint)entityId.GetHashCode() % NumberOfEntityBuckets);
	}

	private void TickNextBucket()
	{
		if (_nextBucketIndex < NumberOfSingletonBuckets)
		{
			_tickableSingletonService.TickAll();
		}
		else
		{
			_tickableEntityBuckets[_nextBucketIndex - NumberOfSingletonBuckets].TickAll();
		}
		MoveNextTickedBucketIndex();
	}

	private void MoveNextTickedBucketIndex()
	{
		_nextBucketIndex = NextBucketIndex(_nextBucketIndex);
	}

	private int NextBucketIndex(int bucketIndex)
	{
		if (++bucketIndex < TotalNumberOfBuckets)
		{
			return bucketIndex;
		}
		return 0;
	}
}

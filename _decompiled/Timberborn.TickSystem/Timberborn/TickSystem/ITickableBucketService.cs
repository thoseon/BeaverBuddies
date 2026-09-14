namespace Timberborn.TickSystem;

internal interface ITickableBucketService
{
	int TotalNumberOfBuckets { get; }

	void AddEntity(TickableEntity tickableEntity);

	void RemoveEntity(TickableEntity tickableEntity);

	void TickBuckets(int numberOfBucketsToTick);

	void FinishFullTick();

	void TickOnce();
}

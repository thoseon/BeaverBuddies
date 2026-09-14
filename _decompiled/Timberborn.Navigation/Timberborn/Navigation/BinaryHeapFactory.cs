namespace Timberborn.Navigation;

internal class BinaryHeapFactory
{
	private readonly NodeIdService _nodeIdService;

	private int InitialCapacity => _nodeIdService.NumberOfNodes / 4;

	public BinaryHeapFactory(NodeIdService nodeIdService)
	{
		_nodeIdService = nodeIdService;
	}

	public BinaryHeap<TValue> Create<TValue>() where TValue : IOrderable<TValue>
	{
		return new BinaryHeap<TValue>(InitialCapacity);
	}
}

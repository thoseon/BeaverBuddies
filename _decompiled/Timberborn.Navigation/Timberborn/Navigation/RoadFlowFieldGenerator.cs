using Timberborn.Common;
using Timberborn.SingletonSystem;

namespace Timberborn.Navigation;

internal class RoadFlowFieldGenerator : ILoadableSingleton
{
	private readonly struct Node : IOrderable<Node>
	{
		public int NodeId { get; }

		public float Distance { get; }

		public Node(int nodeId, float distance)
		{
			NodeId = nodeId;
			Distance = distance;
		}

		public bool IsLessThan(Node other)
		{
			return Distance < other.Distance;
		}
	}

	private readonly BinaryHeapFactory _binaryHeapFactory;

	private BinaryHeap<Node> _openSet = new BinaryHeap<Node>();

	private AccessFlowField _flowField;

	private AccessFlowField _limitingFlowField;

	public RoadFlowFieldGenerator(BinaryHeapFactory binaryHeapFactory)
	{
		_binaryHeapFactory = binaryHeapFactory;
	}

	public void Load()
	{
		_openSet = _binaryHeapFactory.Create<Node>();
	}

	public void FillFlowField(RoadNavMeshGraph roadNavMeshGraph, AccessFlowField flowField, AccessFlowField limitingFlowField, int startNodeId)
	{
		_flowField = flowField;
		_limitingFlowField = limitingFlowField;
		if (_flowField.IsFilled)
		{
			return;
		}
		_flowField.Clear();
		if (roadNavMeshGraph.IsOnNavMesh(startNodeId) && limitingFlowField.HasNode(startNodeId))
		{
			_openSet.Clear();
			PushNode(startNodeId, 0f);
			while (!_openSet.IsEmpty())
			{
				VisitNeighbors(roadNavMeshGraph, _openSet.Pop());
			}
			_flowField.MarkAsFilled();
		}
	}

	private void VisitNeighbors(RoadNavMeshGraph roadNavMeshGraph, Node node)
	{
		ReadOnlyList<NavMeshNode> neighbors = roadNavMeshGraph.GetNeighbors(node.NodeId);
		for (int i = 0; i < neighbors.Count; i++)
		{
			VisitNode(node, neighbors[i]);
		}
	}

	private void VisitNode(Node parentNode, NavMeshNode node)
	{
		int id = node.Id;
		if (!_flowField.HasNode(id) && _limitingFlowField.HasNode(id))
		{
			float distance = parentNode.Distance + node.Cost;
			PushNode(id, distance, parentNode.NodeId);
		}
	}

	private void PushNode(int nodeId, float distance, int parentNodeId = -1)
	{
		_openSet.Push(new Node(nodeId, distance));
		_flowField.AddNode(nodeId, parentNodeId, distance);
	}
}

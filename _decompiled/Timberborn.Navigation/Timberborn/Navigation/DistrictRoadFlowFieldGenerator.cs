using Timberborn.Common;
using Timberborn.SingletonSystem;

namespace Timberborn.Navigation;

internal class DistrictRoadFlowFieldGenerator : ILoadableSingleton
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

	private BinaryHeap<Node> _openSet;

	private AccessFlowField _flowField;

	public DistrictRoadFlowFieldGenerator(BinaryHeapFactory binaryHeapFactory)
	{
		_binaryHeapFactory = binaryHeapFactory;
	}

	public void Load()
	{
		_openSet = _binaryHeapFactory.Create<Node>();
	}

	public void FillFlowFieldUpToDistance(RoadNavMeshGraph roadNavMeshGraph, DistrictObstacleService districtObstacleService, AccessFlowField flowField, int startNodeId)
	{
		_flowField = flowField;
		if (_flowField.IsFilled)
		{
			return;
		}
		_flowField.Clear();
		if (roadNavMeshGraph.IsOnNavMesh(startNodeId))
		{
			_openSet.Clear();
			PushNode(startNodeId, 0f);
			while (!_openSet.IsEmpty())
			{
				VisitNeighbors(roadNavMeshGraph, districtObstacleService, _openSet.Pop());
			}
			_flowField.MarkAsFilled();
		}
	}

	private void VisitNeighbors(RoadNavMeshGraph roadNavMeshGraph, DistrictObstacleService districtObstacleService, Node node)
	{
		ReadOnlyList<NavMeshNode> neighbors = roadNavMeshGraph.GetNeighbors(node.NodeId);
		for (int i = 0; i < neighbors.Count; i++)
		{
			VisitNode(districtObstacleService, node, neighbors[i]);
		}
	}

	private void VisitNode(DistrictObstacleService districtObstacleService, Node parentNode, NavMeshNode node)
	{
		int id = node.Id;
		if (!_flowField.HasNode(id) && !districtObstacleService.IsSetObstacle(id))
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

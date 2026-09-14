using Timberborn.Common;
using Timberborn.SingletonSystem;

namespace Timberborn.Navigation;

internal class TerrainFlowFieldGenerator : ILoadableSingleton
{
	private readonly struct Node : IOrderable<Node>
	{
		public int NodeId { get; }

		public int ParentNodeId { get; }

		public float Distance { get; }

		public float SimpleDistance { get; }

		public Node(int nodeId, int parentNodeId, float distance, float simpleDistance)
		{
			NodeId = nodeId;
			ParentNodeId = parentNodeId;
			Distance = distance;
			SimpleDistance = simpleDistance;
		}

		public bool IsLessThan(Node other)
		{
			return Distance < other.Distance;
		}
	}

	private readonly BinaryHeapFactory _binaryHeapFactory;

	private readonly NavMeshGroupService _navMeshGroupService;

	private BinaryHeap<Node> _openSet;

	private AccessFlowField _flowField;

	private float _maxDistance;

	private int _defaultGroupId;

	public TerrainFlowFieldGenerator(BinaryHeapFactory binaryHeapFactory, NavMeshGroupService navMeshGroupService)
	{
		_binaryHeapFactory = binaryHeapFactory;
		_navMeshGroupService = navMeshGroupService;
	}

	public void Load()
	{
		_openSet = _binaryHeapFactory.Create<Node>();
		_defaultGroupId = _navMeshGroupService.GetDefaultGroupId();
	}

	public void FillFlowFieldUpToDistance(TerrainNavMeshGraph terrainNavMeshGraph, AccessFlowField flowField, float maxDistance, int startNodeId)
	{
		_flowField = flowField;
		_maxDistance = maxDistance;
		if (_flowField.IsFilled)
		{
			return;
		}
		_flowField.Clear();
		if (!terrainNavMeshGraph.IsOnNavMesh(startNodeId))
		{
			return;
		}
		_openSet.Clear();
		PushNode(startNodeId, 0f, 0f);
		while (!_openSet.IsEmpty())
		{
			Node node = _openSet.Pop();
			int nodeId = node.NodeId;
			if (!_flowField.HasNode(nodeId))
			{
				_flowField.AddNode(nodeId, node.ParentNodeId, node.Distance);
				VisitNeighbors(terrainNavMeshGraph, node);
			}
		}
		_flowField.MarkAsFilled();
	}

	private void VisitNeighbors(TerrainNavMeshGraph terrainNavMeshGraph, Node node)
	{
		ReadOnlyList<NavMeshNode> neighbors = terrainNavMeshGraph.GetNeighbors(node.NodeId);
		for (int i = 0; i < neighbors.Count; i++)
		{
			VisitNode(node, neighbors[i]);
		}
	}

	private void VisitNode(Node parentNode, NavMeshNode node)
	{
		int id = node.Id;
		if (!_flowField.HasNode(id))
		{
			float num = ((node.GroupId == _defaultGroupId) ? ((float)((node.Cost > 0f) ? 1 : 0)) : node.Cost);
			float num2 = ((node.Cost >= (float)NavigationLimits.MaxEdgeCost) ? ((float)NavigationLimits.MaxEdgeCost) : num);
			float num3 = parentNode.SimpleDistance + num2;
			if (num3 <= _maxDistance)
			{
				float distance = parentNode.Distance + node.Cost;
				PushNode(id, distance, num3, parentNode.NodeId);
			}
		}
	}

	private void PushNode(int nodeId, float distance, float simpleDistance, int parentNodeId = -1)
	{
		_openSet.Push(new Node(nodeId, parentNodeId, distance, simpleDistance));
	}
}

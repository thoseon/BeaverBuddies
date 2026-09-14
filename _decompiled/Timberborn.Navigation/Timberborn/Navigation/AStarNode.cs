namespace Timberborn.Navigation;

internal readonly struct AStarNode : IOrderable<AStarNode>
{
	private readonly float _fScore;

	public int NodeId { get; }

	public int ParentNodeId { get; }

	public float GScore { get; }

	public AStarNode(int nodeId, int parentNodeId, float gScore, float fScore)
	{
		NodeId = nodeId;
		ParentNodeId = parentNodeId;
		GScore = gScore;
		_fScore = fScore;
	}

	public bool IsLessThan(AStarNode other)
	{
		return _fScore < other._fScore;
	}
}

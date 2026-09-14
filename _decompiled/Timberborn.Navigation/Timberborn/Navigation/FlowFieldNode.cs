namespace Timberborn.Navigation;

internal readonly struct FlowFieldNode
{
	public int Id { get; }

	public float GScore { get; }

	public FlowFieldNode(int id, float gScore)
	{
		Id = id;
		GScore = gScore;
	}
}

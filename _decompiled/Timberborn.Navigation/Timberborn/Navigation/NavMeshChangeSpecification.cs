namespace Timberborn.Navigation;

internal readonly struct NavMeshChangeSpecification
{
	public NavMeshEdge NavMeshEdge { get; }

	public NavMeshChangeType NavMeshChangeType { get; }

	public NavMeshChangeSpecification(NavMeshEdge navMeshEdge, NavMeshChangeType navMeshChangeType)
	{
		NavMeshEdge = navMeshEdge;
		NavMeshChangeType = navMeshChangeType;
	}
}

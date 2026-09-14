namespace Timberborn.Navigation;

internal class NavMeshObjectFactory : INavMeshObjectFactory
{
	private readonly NavMeshUpdater _navMeshUpdater;

	private readonly RestrictedNodeUpdater _restrictedNodeUpdater;

	public NavMeshObjectFactory(NavMeshUpdater navMeshUpdater, RestrictedNodeUpdater restrictedNodeUpdater)
	{
		_navMeshUpdater = navMeshUpdater;
		_restrictedNodeUpdater = restrictedNodeUpdater;
	}

	public NavMeshObject Create()
	{
		return new NavMeshObject(_navMeshUpdater, _restrictedNodeUpdater);
	}
}

using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Navigation;

namespace Timberborn.BlockSystemNavigation;

internal class BlockObjectNavMesh : BaseComponent, IAwakableComponent, IBlockObjectNavMesh
{
	private readonly INavMeshObjectFactory _navMeshObjectFactory;

	private readonly NavMeshObjectUpdater _navMeshObjectUpdater;

	private BlockObject _blockObject;

	private BlockObjectNavMeshSettingsSpec _blockObjectNavMeshSettingsSpec;

	public NavMeshObject NavMeshObject { get; private set; }

	public BlockObjectNavMesh(INavMeshObjectFactory navMeshObjectFactory, NavMeshObjectUpdater navMeshObjectUpdater)
	{
		_navMeshObjectFactory = navMeshObjectFactory;
		_navMeshObjectUpdater = navMeshObjectUpdater;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_blockObjectNavMeshSettingsSpec = GetComponent<BlockObjectNavMeshSettingsSpec>();
		NavMeshObject = _navMeshObjectFactory.Create();
	}

	public void RecalculateNavMeshObject()
	{
		_navMeshObjectUpdater.Update(_blockObject, NavMeshObject, _blockObjectNavMeshSettingsSpec);
	}
}

using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;

namespace Timberborn.BlockSystemNavigation;

internal class BlockObjectNavMeshAdder : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity
{
	private BlockObject _blockObject;

	private BlockObjectNavMesh _blockObjectNavMesh;

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_blockObjectNavMesh = GetComponent<BlockObjectNavMesh>();
	}

	public void InitializeEntity()
	{
		if (!_blockObject.IsPreview)
		{
			_blockObjectNavMesh.RecalculateNavMeshObject();
			_blockObjectNavMesh.NavMeshObject.EnqueueAddToRegularNavMesh();
		}
	}

	public void DeleteEntity()
	{
		if (!_blockObject.IsPreview)
		{
			_blockObjectNavMesh.NavMeshObject.EnqueueRemoveFromRegularNavMesh();
		}
	}
}

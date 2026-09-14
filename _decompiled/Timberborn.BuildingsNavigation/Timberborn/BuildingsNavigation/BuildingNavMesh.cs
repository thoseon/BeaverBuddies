using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockSystemNavigation;
using Timberborn.Navigation;

namespace Timberborn.BuildingsNavigation;

public class BuildingNavMesh : BaseComponent, IAwakableComponent, IFinishedStateListener, IUnfinishedStateListener
{
	private IBlockObjectNavMesh _blockObjectNavMesh;

	private bool _isAddedToRegularNavMesh;

	private bool _isAddedToPreviewNavMesh;

	private bool _isBlocked;

	private NavMeshObject NavMeshObject => _blockObjectNavMesh.NavMeshObject;

	public void Awake()
	{
		_blockObjectNavMesh = GetComponent<IBlockObjectNavMesh>();
	}

	public void OnEnterFinishedState()
	{
		if (!_isBlocked)
		{
			AddToNavMesh();
		}
	}

	public void OnExitFinishedState()
	{
		if (!_isBlocked)
		{
			RemoveFromNavMesh();
		}
	}

	public void OnEnterUnfinishedState()
	{
		if (!_isAddedToPreviewNavMesh)
		{
			RecalculateNavMeshObject();
			NavMeshObject.EnqueueAddToPreviewNavMesh();
			_isAddedToPreviewNavMesh = true;
		}
	}

	public void OnExitUnfinishedState()
	{
		if (_isAddedToPreviewNavMesh)
		{
			NavMeshObject.EnqueueRemoveFromPreviewNavMesh();
		}
	}

	public void UnblockAndAddToNavMesh()
	{
		_isBlocked = false;
		AddToNavMesh();
	}

	public void BlockAndRemoveFromNavMesh()
	{
		_isBlocked = true;
		RemoveFromNavMesh();
	}

	private void AddToNavMesh()
	{
		if (!_isAddedToRegularNavMesh)
		{
			RecalculateNavMeshObject();
			NavMeshObject.EnqueueAddToRegularNavMesh();
			_isAddedToRegularNavMesh = true;
		}
	}

	private void RemoveFromNavMesh()
	{
		if (_isAddedToRegularNavMesh)
		{
			NavMeshObject.EnqueueRemoveFromRegularNavMesh();
			_isAddedToRegularNavMesh = false;
		}
	}

	private void RecalculateNavMeshObject()
	{
		_blockObjectNavMesh.RecalculateNavMeshObject();
	}
}

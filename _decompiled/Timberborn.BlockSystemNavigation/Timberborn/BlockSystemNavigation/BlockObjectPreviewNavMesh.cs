using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;

namespace Timberborn.BlockSystemNavigation;

internal class BlockObjectPreviewNavMesh : BaseComponent, IAwakableComponent, IPreviewServiceMember
{
	private BlockObjectNavMesh _blockObjectNavMesh;

	private bool _isActivePreview;

	public void Awake()
	{
		_blockObjectNavMesh = GetComponent<BlockObjectNavMesh>();
	}

	public void AddToPreviewService()
	{
		RemoveFromPreviewService();
		_blockObjectNavMesh.RecalculateNavMeshObject();
		_blockObjectNavMesh.NavMeshObject.AddToPreviewNavMesh();
	}

	public void RemoveFromPreviewService()
	{
		_blockObjectNavMesh.NavMeshObject?.RemoveFromPreviewNavMesh();
	}
}

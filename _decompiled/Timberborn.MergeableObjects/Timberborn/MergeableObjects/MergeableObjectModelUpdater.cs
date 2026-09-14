using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.MergeableObjects;

internal class MergeableObjectModelUpdater : BaseComponent, IAwakableComponent, IPrePlacementChangeListener, IPostPlacementChangeListener, IPreviewSelectionListener
{
	private readonly IBlockService _blockService;

	private readonly PreviewBlockService _previewBlockService;

	private BlockObject _blockObject;

	public MergeableObjectModelUpdater(IBlockService blockService, PreviewBlockService previewBlockService)
	{
		_blockService = blockService;
		_previewBlockService = previewBlockService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void OnPrePlacementChanged()
	{
		UpdateNeighbors();
	}

	public void OnPostPlacementChanged()
	{
		UpdateNeighbors();
	}

	public void OnPreviewSelect()
	{
		UpdateNeighbors();
	}

	public void OnPreviewUnselect()
	{
		UpdateNeighbors();
	}

	private void UpdateNeighbors()
	{
		if (_blockObject.Positioned)
		{
			Vector3Int vector3Int = _blockObject.PositionedBlocks.GetOccupiedCoordinates().First();
			Vector3Int[] neighbors4Vector3Int = Deltas.Neighbors4Vector3Int;
			foreach (Vector3Int vector3Int2 in neighbors4Vector3Int)
			{
				UpdateNeighbor(vector3Int + vector3Int2);
			}
		}
	}

	private void UpdateNeighbor(Vector3Int target)
	{
		_blockService.GetBottomObjectComponentAt<MergeableObjectModel>(target)?.UpdateModel();
		_previewBlockService.GetBottomObjectComponentAt<MergeableObjectModel>(target)?.UpdateModel();
	}
}

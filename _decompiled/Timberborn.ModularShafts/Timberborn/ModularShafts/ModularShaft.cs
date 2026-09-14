using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.ModularShafts;

internal class ModularShaft : BaseComponent, IAwakableComponent, IPrePlacementChangeListener, IPostPlacementChangeListener, IPreviewSelectionListener, IFinishedStateListener
{
	private readonly IBlockService _blockService;

	private readonly PreviewBlockService _previewBlockService;

	private BlockObject _blockObject;

	public ModularShaft(IBlockService blockService, PreviewBlockService previewBlockService)
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

	public void OnEnterFinishedState()
	{
		UpdateNeighbors();
	}

	public void OnExitFinishedState()
	{
	}

	private void UpdateNeighbors()
	{
		if (_blockObject.Positioned)
		{
			Vector3Int[] neighbors6Vector3Int = Deltas.Neighbors6Vector3Int;
			foreach (Vector3Int vector3Int in neighbors6Vector3Int)
			{
				UpdateNeighbor(_blockObject.Coordinates + vector3Int);
			}
		}
	}

	private void UpdateNeighbor(Vector3Int position)
	{
		_blockService.GetFirstObjectWithComponentAt<BlockObjectModelController>(position)?.UpdateModel();
		_previewBlockService.GetFirstObjectWithComponentAt<BlockObjectModelController>(position)?.UpdateModel();
	}
}

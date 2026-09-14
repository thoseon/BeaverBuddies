using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using UnityEngine;

namespace Timberborn.WaterBuildings;

internal class PreviewWaterInputPipeBlocker : BaseComponent, IAwakableComponent, IPrePlacementChangeListener, IPostPlacementChangeListener, IPreviewSelectionListener
{
	private readonly PreviewWaterInputPipeBlockerService _previewWaterInputPipeBlockerService;

	private BlockObject _blockObject;

	private PipeIntersectionAllowerSpec _pipeIntersectionAllowerSpec;

	private bool _isBlocking;

	public PreviewWaterInputPipeBlocker(PreviewWaterInputPipeBlockerService previewWaterInputPipeBlockerService)
	{
		_previewWaterInputPipeBlockerService = previewWaterInputPipeBlockerService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_pipeIntersectionAllowerSpec = GetComponent<PipeIntersectionAllowerSpec>();
	}

	public void OnPrePlacementChanged()
	{
		Unblock();
	}

	public void OnPostPlacementChanged()
	{
		Block();
	}

	public void OnPreviewSelect()
	{
		Block();
	}

	public void OnPreviewUnselect()
	{
		Unblock();
	}

	private void Block()
	{
		if (IsValidBlocker() && !_isBlocking)
		{
			_previewWaterInputPipeBlockerService.Block(GetBlockingOccupiedTiles());
			_isBlocking = true;
		}
	}

	private void Unblock()
	{
		if (IsValidBlocker() && _isBlocking)
		{
			_previewWaterInputPipeBlockerService.Unblock(GetBlockingOccupiedTiles());
			_isBlocking = false;
		}
	}

	private bool IsValidBlocker()
	{
		if (_blockObject.IsPreview)
		{
			return _pipeIntersectionAllowerSpec == null;
		}
		return false;
	}

	private IEnumerable<Vector3Int> GetBlockingOccupiedTiles()
	{
		return _blockObject.PositionedBlocks.GetOccupiedCoordinatesIntersecting(WaterInputPipeCoordinates.InvalidOccupations);
	}
}

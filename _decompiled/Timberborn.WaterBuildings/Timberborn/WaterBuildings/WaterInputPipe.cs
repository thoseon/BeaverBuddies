using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.WaterBuildings;

internal class WaterInputPipe : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity
{
	private readonly EventBus _eventBus;

	private readonly PreviewWaterInputPipeBlockerService _previewWaterInputPipeBlockerService;

	private WaterInputPipeCoordinates _waterInputPipeCoordinates;

	private BlockObject _blockObject;

	private BlockObjectModelController _blockObjectModelController;

	private HighlightableObject _highlightableObject;

	private WaterInputPipeSegmentCreator _waterInputPipeSegmentCreator;

	private readonly List<PipeSegment> _pipeSegments = new List<PipeSegment>();

	public WaterInputPipe(EventBus eventBus, PreviewWaterInputPipeBlockerService previewWaterInputPipeBlockerService)
	{
		_eventBus = eventBus;
		_previewWaterInputPipeBlockerService = previewWaterInputPipeBlockerService;
	}

	public void Awake()
	{
		_waterInputPipeCoordinates = GetComponent<WaterInputPipeCoordinates>();
		_blockObject = GetComponent<BlockObject>();
		_blockObjectModelController = GetComponent<BlockObjectModelController>();
		_highlightableObject = GetComponent<HighlightableObject>();
		_waterInputPipeSegmentCreator = GetComponent<WaterInputPipeSegmentCreator>();
		_waterInputPipeCoordinates.CoordinatesChanged += OnWaterPipeCoordinatesChanged;
	}

	public void InitializeEntity()
	{
		_eventBus.Register(this);
		UpdatePipe();
	}

	public void DeleteEntity()
	{
		_eventBus.Unregister(this);
	}

	[OnEvent]
	public void OnPreviewPipeBlockingCoordinatesChanged(PreviewBlockingCoordinatesChangedEvent e)
	{
		if (ShouldUpdatePipe(e.ChangedCoordinates))
		{
			UpdatePipe();
		}
	}

	private void OnWaterPipeCoordinatesChanged(object sender, Vector3Int coordinates)
	{
		UpdatePipe();
	}

	private bool ShouldUpdatePipe(ReadOnlyList<Vector3Int> changedCoordinates)
	{
		foreach (Vector3Int item in changedCoordinates)
		{
			if (item.XY() == _waterInputPipeCoordinates.Coordinates.XY())
			{
				return true;
			}
		}
		return false;
	}

	private void UpdatePipe()
	{
		int i;
		for (i = 0; i < _waterInputPipeCoordinates.Depth; i++)
		{
			Vector3Int vector3Int = new Vector3Int(0, 0, _waterInputPipeCoordinates.Depth - i - 1);
			Vector3Int gridPosition = _waterInputPipeCoordinates.Coordinates + vector3Int;
			if (!CanShowPipeSegment(gridPosition))
			{
				break;
			}
			ShowPipeSegment(i, gridPosition);
		}
		DisablePipeSegmentsAfter(i);
		_blockObjectModelController.UpdateAll();
		_highlightableObject.RefreshHighlight();
	}

	private bool CanShowPipeSegment(Vector3Int gridPosition)
	{
		if (!_blockObject.IsPreview)
		{
			return !_previewWaterInputPipeBlockerService.IsBlocked(gridPosition);
		}
		return true;
	}

	private void ShowPipeSegment(int index, Vector3Int gridPosition)
	{
		PipeSegment orCreatePipeSegment = GetOrCreatePipeSegment(index);
		Vector3 position = CoordinateSystem.GridToWorldCentered(gridPosition);
		if (IsEndSegment(index, gridPosition))
		{
			orCreatePipeSegment.ShowEnd(position);
		}
		else
		{
			orCreatePipeSegment.ShowMiddle(position);
		}
	}

	private PipeSegment GetOrCreatePipeSegment(int index)
	{
		if (index >= _pipeSegments.Count)
		{
			PipeSegment item = (_blockObject.IsFinished ? _waterInputPipeSegmentCreator.CreateFinished() : _waterInputPipeSegmentCreator.CreateUnfinished());
			_pipeSegments.Add(item);
		}
		return _pipeSegments[index];
	}

	private bool IsEndSegment(int index, Vector3Int gridPosition)
	{
		Vector3Int coordinates = gridPosition - new Vector3Int(0, 0, 1);
		if (!_previewWaterInputPipeBlockerService.IsBlocked(coordinates))
		{
			return index == _waterInputPipeCoordinates.Depth - 1;
		}
		return true;
	}

	private void DisablePipeSegmentsAfter(int index)
	{
		for (int i = index; i < _pipeSegments.Count; i++)
		{
			_pipeSegments[i].Hide();
		}
	}
}

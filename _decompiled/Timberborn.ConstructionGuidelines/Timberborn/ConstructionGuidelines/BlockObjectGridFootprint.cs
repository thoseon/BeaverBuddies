using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using UnityEngine;

namespace Timberborn.ConstructionGuidelines;

internal class BlockObjectGridFootprint : BaseComponent, IAwakableComponent, IPostPlacementChangeListener, IPreviewSelectionListener
{
	private readonly Dictionary<Vector2Int, FootprintCoordinates> _footprintsCoordinatesPerCell = new Dictionary<Vector2Int, FootprintCoordinates>();

	private readonly ConstructionGuidelinesRenderingService _constructionGuidelinesRenderingService;

	private BlockObject _blockObject;

	private BlockObjectCenter _blockObjectCenter;

	private Preview _preview;

	private Vector2Int _min;

	private Vector2Int _max;

	public BlockObjectGridFootprint(ConstructionGuidelinesRenderingService constructionGuidelinesRenderingService)
	{
		_constructionGuidelinesRenderingService = constructionGuidelinesRenderingService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_blockObjectCenter = GetComponent<BlockObjectCenter>();
		_preview = GetComponent<Preview>();
	}

	public void OnPostPlacementChanged()
	{
		if (!_blockObject.IsPreview)
		{
			return;
		}
		_min = new Vector2Int(int.MaxValue, int.MaxValue);
		_max = new Vector2Int(int.MinValue, int.MinValue);
		_footprintsCoordinatesPerCell.Clear();
		foreach (Block occupiedBlock in _blockObject.PositionedBlocks.GetOccupiedBlocks())
		{
			Vector2Int coords2D = new Vector2Int(occupiedBlock.Coordinates.x, occupiedBlock.Coordinates.y);
			UpdateLowestCoordinatePerCell(occupiedBlock, coords2D);
			_min = new Vector2Int(Math.Min(_min.x, coords2D.x), Math.Min(_min.y, coords2D.y));
			_max = new Vector2Int(Math.Max(_max.x, coords2D.x), Math.Max(_max.y, coords2D.y));
		}
	}

	public void OnPreviewSelect()
	{
		if (_preview.PreviewState.IsLast)
		{
			_constructionGuidelinesRenderingService.SetPreviewFootprint(_min, _max, _blockObjectCenter.GridCenterAtBaseZ, _footprintsCoordinatesPerCell.Values);
		}
	}

	public void OnPreviewUnselect()
	{
		_footprintsCoordinatesPerCell.Clear();
	}

	private void UpdateLowestCoordinatePerCell(Block block, Vector2Int coords2D)
	{
		Vector3Int coordinates = block.Coordinates;
		if (!block.IsOccupied)
		{
			return;
		}
		bool canHaveFootprint = (block.Occupation & (BlockOccupations.Floor | BlockOccupations.Bottom | BlockOccupations.Corners | BlockOccupations.Path | BlockOccupations.Middle)) == 0;
		if (_footprintsCoordinatesPerCell.TryGetValue(coords2D, out var value))
		{
			if (value.Coordinates.z > coordinates.z)
			{
				_footprintsCoordinatesPerCell[coords2D] = new FootprintCoordinates(coordinates, canHaveFootprint);
			}
		}
		else
		{
			_footprintsCoordinatesPerCell.Add(coords2D, new FootprintCoordinates(coordinates, canHaveFootprint));
		}
	}
}

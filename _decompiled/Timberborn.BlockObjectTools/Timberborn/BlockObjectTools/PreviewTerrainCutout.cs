using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.BlockObjectTools;

internal class PreviewTerrainCutout : BaseComponent, IAwakableComponent, IPrePlacementChangeListener, IPostPlacementChangeListener, IPreviewSelectionListener
{
	private readonly ITerrainService _terrainService;

	private readonly PreviewTerrainCutoutService _previewTerrainCutoutService;

	private readonly StackableBlockService _stackableBlockService;

	private BlockObject _blockObject;

	private ICutoutTilesProvider _cutoutTilesProvider;

	private bool _isTerrainCutoutSet;

	public PreviewTerrainCutout(ITerrainService terrainService, PreviewTerrainCutoutService previewTerrainCutoutService, StackableBlockService stackableBlockService)
	{
		_terrainService = terrainService;
		_previewTerrainCutoutService = previewTerrainCutoutService;
		_stackableBlockService = stackableBlockService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_cutoutTilesProvider = GetComponent<ICutoutTilesProvider>();
	}

	public void OnPrePlacementChanged()
	{
		UnsetTerrainCutout();
	}

	public void OnPostPlacementChanged()
	{
		SetTerrainCutout();
	}

	public void OnPreviewSelect()
	{
		_terrainService.TerrainHeightChanged += OnTerrainHeightChanged;
		SetTerrainCutout();
	}

	public void OnPreviewUnselect()
	{
		UnsetTerrainCutout();
		_terrainService.TerrainHeightChanged -= OnTerrainHeightChanged;
	}

	private void SetTerrainCutout()
	{
		if (_blockObject.IsPreview && !_isTerrainCutoutSet)
		{
			_previewTerrainCutoutService.SetCutout(GetCutoutTiles());
			_isTerrainCutoutSet = true;
		}
	}

	private void UnsetTerrainCutout()
	{
		if (_blockObject.IsPreview && _isTerrainCutoutSet)
		{
			_previewTerrainCutoutService.UnsetCutout(GetCutoutTiles());
			_isTerrainCutoutSet = false;
		}
	}

	private IEnumerable<Vector3Int> GetCutoutTiles()
	{
		int baseHeight = _blockObject.CoordinatesAtBaseZ.z;
		foreach (Vector3Int positionedCutoutTile in _cutoutTilesProvider.GetPositionedCutoutTiles())
		{
			if (positionedCutoutTile.z == baseHeight || _stackableBlockService.IsUnfinishedGroundBlockAt(positionedCutoutTile.Below()))
			{
				yield return positionedCutoutTile;
			}
		}
	}

	private void OnTerrainHeightChanged(object sender, TerrainHeightChangeEventArgs terrainHeightChangeEventArgs)
	{
		TerrainHeightChange change = terrainHeightChangeEventArgs.Change;
		if (!_isTerrainCutoutSet)
		{
			return;
		}
		Vector3Int vector3Int = change.Coordinates.ToVector3Int(change.To);
		if (CutoutContainsCoordinates(vector3Int))
		{
			if (_blockObject.CoordinatesAtBaseZ.z == change.To)
			{
				_previewTerrainCutoutService.SetCutout(vector3Int);
			}
			else
			{
				_previewTerrainCutoutService.UnsetCutout(vector3Int);
			}
		}
	}

	private bool CutoutContainsCoordinates(Vector3Int coordinates)
	{
		foreach (Vector3Int positionedCutoutTile in _cutoutTilesProvider.GetPositionedCutoutTiles())
		{
			if (positionedCutoutTile == coordinates)
			{
				return true;
			}
		}
		return false;
	}
}

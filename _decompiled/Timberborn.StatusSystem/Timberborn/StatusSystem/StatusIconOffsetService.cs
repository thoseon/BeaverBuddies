using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.StatusSystem;

internal class StatusIconOffsetService : IStatusIconOffsetService, ILoadableSingleton
{
	private static readonly Vector2[] Offsets = new Vector2[9]
	{
		new Vector2(0f, 0f),
		new Vector2(0f, 0.5f),
		new Vector2(0.5f, 0.5f),
		new Vector2(0.5f, 0f),
		new Vector2(0.5f, -0.5f),
		new Vector2(0f, -0.5f),
		new Vector2(-0.5f, -0.5f),
		new Vector2(-0.5f, 0f),
		new Vector2(-0.5f, 0.5f)
	};

	private readonly ITerrainService _terrainService;

	private readonly StatusSlotUpdateService _statusSlotUpdateService;

	private readonly StatusIconOffsetCalculator _statusIconOffsetCalculator;

	private readonly Dictionary<Vector2Int, List<IStatusIconOffsetter>> _offsetters = new Dictionary<Vector2Int, List<IStatusIconOffsetter>>();

	private readonly HashSet<Vector2Int> _affectedKeysCache = new HashSet<Vector2Int>();

	private bool _previewMode;

	public StatusIconOffsetService(ITerrainService terrainService, StatusSlotUpdateService statusSlotUpdateService, StatusIconOffsetCalculator statusIconOffsetCalculator)
	{
		_terrainService = terrainService;
		_statusSlotUpdateService = statusSlotUpdateService;
		_statusIconOffsetCalculator = statusIconOffsetCalculator;
	}

	public void Load()
	{
		_terrainService.TerrainHeightChanged += OnTerrainHeightChanged;
	}

	public void AddOffsetter(IStatusIconOffsetter offsetter)
	{
		Vector2Int key = offsetter.Key;
		if (_offsetters.TryGetValue(key, out var value))
		{
			value.Add(offsetter);
			return;
		}
		_offsetters[key] = new List<IStatusIconOffsetter> { offsetter };
	}

	public void RemoveOffsetter(IStatusIconOffsetter offsetter)
	{
		Vector2Int key = offsetter.Key;
		if (_offsetters.TryGetValue(key, out var value))
		{
			value.Remove(offsetter);
			if (value.Count > 0)
			{
				UpdateIcons(value.AsReadOnlyList());
			}
			else
			{
				_statusSlotUpdateService.ClearStatusSlots(key);
			}
		}
	}

	public void UpdateAffectedStatusSlot(Vector2Int coordinates)
	{
		GetEffectedKeysForGridPosition(coordinates);
		foreach (Vector2Int item in _affectedKeysCache)
		{
			if (HasOffsetterAt(item, out var readOnlyOffsetters))
			{
				_statusSlotUpdateService.UpdateStatusSlots(item);
				UpdateIcons(readOnlyOffsetters);
			}
		}
		_affectedKeysCache.Clear();
	}

	public void UpdateIcons(IStatusIconOffsetter offsetter)
	{
		GetKeysAffectedByOffsetter(offsetter);
		foreach (Vector2Int item in _affectedKeysCache)
		{
			if (HasOffsetterAt(item, out var readOnlyOffsetters))
			{
				UpdateIcons(readOnlyOffsetters);
			}
		}
		_affectedKeysCache.Clear();
	}

	public void UpdatePositions(IStatusIconOffsetter offsetter)
	{
		GetKeysAffectedByOffsetter(offsetter);
		foreach (Vector2Int item in _affectedKeysCache)
		{
			if (HasOffsetterAt(item, out var _))
			{
				_statusSlotUpdateService.UpdateStatusSlots(item);
			}
		}
		_affectedKeysCache.Clear();
		UpdateIcons(offsetter);
	}

	public float CalculateVerticalPosition(IStatusIconOffsetter offsetter)
	{
		ReadOnlyList<IStatusIconOffsetter> offsetters = _offsetters[offsetter.Key].AsReadOnlyList();
		if (!_previewMode)
		{
			return _statusIconOffsetCalculator.CalculateVerticalPosition(offsetters, offsetter);
		}
		return _statusIconOffsetCalculator.CalculatePreviewVerticalPosition(offsetters, offsetter);
	}

	public void RepositionAllIcons()
	{
		foreach (List<IStatusIconOffsetter> value in _offsetters.Values)
		{
			UpdateIcons(value.AsReadOnlyList());
		}
	}

	public IEnumerable<(StatusSlot, Vector2)> GetAllStatusSlots()
	{
		return _statusSlotUpdateService.GetAllStatusSlots();
	}

	public void EnablePreviewMode()
	{
		_previewMode = true;
	}

	public void DisablePreviewMode()
	{
		_previewMode = false;
	}

	private void OnTerrainHeightChanged(object sender, TerrainHeightChangeEventArgs terrainColumnChangedEventArgs)
	{
		Vector2Int coordinates = terrainColumnChangedEventArgs.Change.Coordinates;
		GetEffectedKeysForGridPosition(coordinates);
		foreach (Vector2Int item in _affectedKeysCache)
		{
			if (HasOffsetterAt(item, out var readOnlyOffsetters))
			{
				_statusSlotUpdateService.UpdateStatusSlots(item);
				UpdateIcons(readOnlyOffsetters);
			}
		}
		_affectedKeysCache.Clear();
	}

	private bool HasOffsetterAt(Vector2Int key, out ReadOnlyList<IStatusIconOffsetter> readOnlyOffsetters)
	{
		if (_offsetters.TryGetValue(key, out var value) && value.Count > 0)
		{
			readOnlyOffsetters = value.AsReadOnlyList();
			return true;
		}
		return false;
	}

	private static void UpdateIcons(ReadOnlyList<IStatusIconOffsetter> offsetters)
	{
		foreach (IStatusIconOffsetter item in offsetters)
		{
			item.UpdateIcon();
		}
	}

	private void GetKeysAffectedByOffsetter(IStatusIconOffsetter offsetter)
	{
		foreach (Vector3Int occupiedCoordinate in offsetter.BlockObject.PositionedBlocks.GetOccupiedCoordinates())
		{
			GetEffectedKeysForGridPosition(occupiedCoordinate.XY());
		}
	}

	private void GetEffectedKeysForGridPosition(Vector2Int position)
	{
		Vector2[] offsets = Offsets;
		for (int i = 0; i < offsets.Length; i++)
		{
			Vector2 vector = offsets[i];
			int x = Mathf.RoundToInt(((float)position.x + vector.x) * 2f) + 1;
			int y = Mathf.RoundToInt(((float)position.y + vector.y) * 2f) + 1;
			_affectedKeysCache.Add(new Vector2Int(x, y));
		}
	}
}

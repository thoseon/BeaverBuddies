using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.StatusSystem;

internal class StatusSlotUpdateService
{
	private readonly struct AffectedCoordinates
	{
		public int XMin { get; }

		public int XMax { get; }

		public int YMin { get; }

		public int YMax { get; }

		public int Z { get; }

		public AffectedCoordinates(Vector2Int key, int z)
		{
			XMin = Mathf.FloorToInt((float)(key.x - 1) / 2f);
			XMax = XMin + (key.x - 1) % 2;
			YMin = Mathf.FloorToInt((float)(key.y - 1) / 2f);
			YMax = YMin + (key.y - 1) % 2;
			Z = z;
		}
	}

	private readonly struct PositionedOccupier
	{
		public StatusSlotOccupier Occupier { get; }

		public Vector3Int Coordinates { get; }

		public PositionedOccupier(StatusSlotOccupier occupier, Vector3Int coordinates)
		{
			Occupier = occupier;
			Coordinates = coordinates;
		}
	}

	private static readonly float SlotZCoordinateOffset = 0.5f;

	private static readonly float TopBoundForTerrain = 1.6f;

	private readonly IBlockService _blockService;

	private readonly ITerrainService _terrainService;

	private readonly StatusIconSlotFactory _statusIconSlotFactory;

	private readonly Dictionary<Vector2Int, List<StatusSlot>> _slots = new Dictionary<Vector2Int, List<StatusSlot>>();

	private readonly Dictionary<int, TopBoundForLayer> _topBoundCache = new Dictionary<int, TopBoundForLayer>();

	private readonly List<PositionedOccupier> _positionedOccupierCache = new List<PositionedOccupier>();

	public StatusSlotUpdateService(IBlockService blockService, ITerrainService terrainService, StatusIconSlotFactory statusIconSlotFactory)
	{
		_blockService = blockService;
		_terrainService = terrainService;
		_statusIconSlotFactory = statusIconSlotFactory;
	}

	public ReadOnlyList<StatusSlot> GetStatusSlots(Vector2Int key)
	{
		if (_slots.TryGetValue(key, out var value) && value.Count > 0)
		{
			return value.AsReadOnlyList();
		}
		UpdateStatusSlots(key);
		return _slots[key].AsReadOnlyList();
	}

	public void UpdateStatusSlots(Vector2Int key)
	{
		List<StatusSlot> orAdd = _slots.GetOrAdd(key);
		orAdd.Clear();
		CalculateSlotPositions(in key, orAdd);
		_topBoundCache.Clear();
	}

	public void ClearStatusSlots(Vector2Int key)
	{
		if (_slots.TryGetValue(key, out var value))
		{
			value.Clear();
		}
	}

	public IEnumerable<(StatusSlot, Vector2)> GetAllStatusSlots()
	{
		foreach (var (key, list2) in _slots)
		{
			foreach (StatusSlot item in list2)
			{
				yield return (item, new Vector2((float)key.x / 2f, (float)key.y / 2f));
			}
		}
	}

	private void CalculateSlotPositions(in Vector2Int key, IList<StatusSlot> statusSlots)
	{
		byte a = byte.MaxValue;
		for (int i = 0; i < _blockService.Size.z * 2; i++)
		{
			float num = SlotZCoordinateOffset * (float)(i + 1);
			SlotConstraints constraints = GetConstraints(key, i, num);
			byte minBaseZ = (byte)Mathf.Min(a, constraints.BaseZ);
			StatusSlot item = CreateStatusIconSlot(constraints, i / 2, num, minBaseZ);
			statusSlots.Add(item);
			a = constraints.BaseZ;
		}
	}

	private SlotConstraints GetConstraints(Vector2Int key, int z, float slotZCoordinate)
	{
		int num = z / 2;
		if (z % 2 == 0)
		{
			BlockOccupations occupation = SlotBlockOccupation.GetOccupation(key, isMiddleSlot: false);
			return GetConstraints(slotZCoordinate, new AffectedCoordinates(key, z / 2), occupation);
		}
		BlockOccupations occupation2 = SlotBlockOccupation.GetOccupation(key, isMiddleSlot: true);
		SlotConstraints constraints = GetConstraints(slotZCoordinate, new AffectedCoordinates(key, num), occupation2);
		SlotConstraints constraints2 = GetConstraints(slotZCoordinate, new AffectedCoordinates(key, num + 1), SlotBlockOccupation.Default);
		return constraints.Merge(constraints2);
	}

	private StatusSlot CreateStatusIconSlot(SlotConstraints slotConstraints, int gridZ, float statusZCoordinate, byte minBaseZ)
	{
		int key = gridZ - 1;
		if (_topBoundCache.TryGetValue(key, out var value))
		{
			return _statusIconSlotFactory.CreateBounded(slotConstraints, value, statusZCoordinate, minBaseZ);
		}
		return _statusIconSlotFactory.CreateUnbounded(slotConstraints, statusZCoordinate, minBaseZ);
	}

	private SlotConstraints GetConstraints(float slotZCoordinate, AffectedCoordinates affectedCoordinates, BlockOccupations blockingOccupations)
	{
		int z = affectedCoordinates.Z;
		if (z < _terrainService.Size.z && IsBlockedByTerrain(affectedCoordinates))
		{
			_topBoundCache[z] = new TopBoundForLayer((float)z + TopBoundForTerrain);
			return SlotConstraints.GetOccupied((byte)z);
		}
		return GetConstraintsFromBlockObject(slotZCoordinate, affectedCoordinates, blockingOccupations);
	}

	private bool IsBlockedByTerrain(AffectedCoordinates affectedCoordinates)
	{
		for (int i = affectedCoordinates.XMin; i <= affectedCoordinates.XMax; i++)
		{
			for (int j = affectedCoordinates.YMin; j <= affectedCoordinates.YMax; j++)
			{
				if (_terrainService.Underground(new Vector3Int(i, j, affectedCoordinates.Z)))
				{
					return true;
				}
			}
		}
		return false;
	}

	private SlotConstraints GetConstraintsFromBlockObject(float slotZCoordinate, AffectedCoordinates affectedCoordinates, BlockOccupations occupations)
	{
		GetOccupiersAtAffectedCoordinates(affectedCoordinates);
		byte b = byte.MaxValue;
		bool flag = false;
		bool invalidInConstructionMode = true;
		bool flag2 = true;
		foreach (PositionedOccupier item in _positionedOccupierCache)
		{
			StatusSlotOccupier occupier = item.Occupier;
			Vector3Int coordinates = item.Coordinates;
			if (occupier.IntersectsAt(coordinates, occupations))
			{
				flag = true;
				if (occupier.BaseZ < b)
				{
					b = occupier.BaseZ;
				}
				if (!occupier.IsUnfinished || slotZCoordinate < occupier.GetNormalModeTopBound())
				{
					invalidInConstructionMode = false;
				}
				flag2 = flag2 && occupier.UseUnfinishedConstructionModeModel;
			}
			UpdateTopBoundForLayer(occupier, coordinates);
		}
		_positionedOccupierCache.Clear();
		if (!flag)
		{
			return SlotConstraints.GetUnoccupied(b);
		}
		return SlotConstraints.GetOccupied(b, invalidInConstructionMode, flag2);
	}

	private void GetOccupiersAtAffectedCoordinates(AffectedCoordinates affectedCoordinates)
	{
		for (int i = affectedCoordinates.XMin; i <= affectedCoordinates.XMax; i++)
		{
			for (int j = affectedCoordinates.YMin; j <= affectedCoordinates.YMax; j++)
			{
				Vector3Int coordinates = new Vector3Int(i, j, affectedCoordinates.Z);
				foreach (BlockObject item in _blockService.GetObjectsAt(coordinates))
				{
					StatusSlotOccupier component = item.GetComponent<StatusSlotOccupier>();
					if (component != null)
					{
						_positionedOccupierCache.Add(new PositionedOccupier(component, coordinates));
					}
				}
			}
		}
	}

	private void UpdateTopBoundForLayer(StatusSlotOccupier statusSlotOccupier, Vector3Int coordinates)
	{
		float num = 0f;
		float num2 = 0f;
		if (!_topBoundCache.TryGetValue(coordinates.z, out var value))
		{
			num = value.ConstructionModeTopBound;
			num2 = value.NormalModeTopBound;
		}
		TopBoundForLayer topBound = statusSlotOccupier.GetTopBound(coordinates);
		if (topBound.ConstructionModeTopBound > num)
		{
			num = topBound.ConstructionModeTopBound;
		}
		if (topBound.NormalModeTopBound > num2)
		{
			num2 = topBound.NormalModeTopBound;
		}
		if (num > 0f)
		{
			_topBoundCache[coordinates.z] = new TopBoundForLayer(num, num2);
		}
	}
}

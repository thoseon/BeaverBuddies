using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.MapEditorTickSystem;
using Timberborn.MapIndexSystem;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;

namespace Timberborn.TerrainSystem;

[MapEditorTickable]
internal class ThreadSafeColumnTerrainMap : ILoadableSingleton, ITickableSingleton, IThreadSafeColumnTerrainMap
{
	private readonly struct ColumnChange
	{
		public bool Added { get; }

		public int Index { get; }

		public int ColumnCount { get; }

		public ColumnChange(bool added, int index, int columnCount)
		{
			Added = added;
			Index = index;
			ColumnCount = columnCount;
		}
	}

	private readonly ColumnTerrainMap _columnTerrainMap;

	private readonly MapIndexService _mapIndexService;

	private readonly ITickableSingletonService _tickableSingletonService;

	private int _verticalStride;

	private byte[] _columnCounts;

	private ReadOnlyTerrainColumn[] _terrainColumns;

	private readonly Queue<ColumnChange> _columnChanges = new Queue<ColumnChange>();

	public int MaxColumnCount { get; private set; }

	public ReadOnlyArray<byte> ColumnCounts => new ReadOnlyArray<byte>(_columnCounts);

	public ReadOnlyArray<ReadOnlyTerrainColumn> TerrainColumns => new ReadOnlyArray<ReadOnlyTerrainColumn>(_terrainColumns);

	public event EventHandler<int> ColumnMovedUp;

	public event EventHandler<int> ColumnMovedDown;

	public event EventHandler<int> ColumnReset;

	public event EventHandler<int> MaxTerrainColumnCountChanged;

	public ThreadSafeColumnTerrainMap(ColumnTerrainMap columnTerrainMap, MapIndexService mapIndexService, ITickableSingletonService tickableSingletonService)
	{
		_columnTerrainMap = columnTerrainMap;
		_mapIndexService = mapIndexService;
		_tickableSingletonService = tickableSingletonService;
	}

	public void Load()
	{
		_verticalStride = _mapIndexService.VerticalStride;
		_columnCounts = new byte[_mapIndexService.MaxIndex];
		_terrainColumns = new ReadOnlyTerrainColumn[_verticalStride * _columnTerrainMap.MaxColumnCount];
		MaxColumnCount = _columnTerrainMap.MaxColumnCount;
		UpdateData();
		_columnTerrainMap.ColumnAdded += OnColumnAdded;
		_columnTerrainMap.ColumnRemoved += OnColumnRemoved;
		_tickableSingletonService.ForcedParallelTickFinished += delegate
		{
			UpdateDataAndNotify();
		};
	}

	public void Tick()
	{
		UpdateDataAndNotify();
	}

	public int GetColumnCount(int index2D)
	{
		return _columnCounts[index2D];
	}

	public int GetColumnCeiling(int index3D)
	{
		return _terrainColumns[index3D].Ceiling;
	}

	public int GetColumnFloor(int index3D)
	{
		return _terrainColumns[index3D].Floor;
	}

	public bool TryGetIndexAtCeiling(int index2D, int ceiling, out int index3D)
	{
		byte b = _columnCounts[index2D];
		for (int i = 0; i < b; i++)
		{
			index3D = i * _verticalStride + index2D;
			if (_terrainColumns[index3D].Ceiling == ceiling)
			{
				return true;
			}
		}
		index3D = 0;
		return false;
	}

	public bool TryGetIndexAtOrAboveCeiling(int index2D, int ceiling, out int index3D)
	{
		byte b = _columnCounts[index2D];
		for (int i = 0; i < b; i++)
		{
			index3D = i * _verticalStride + index2D;
			if (_terrainColumns[index3D].Ceiling >= ceiling)
			{
				return true;
			}
		}
		index3D = -1;
		return false;
	}

	private void OnColumnAdded(object sender, ColumnAddedEventArgs e)
	{
		_columnChanges.Enqueue(new ColumnChange(added: true, e.Index, e.ColumnCount));
	}

	private void OnColumnRemoved(object sender, ColumnRemovedEventArgs e)
	{
		_columnChanges.Enqueue(new ColumnChange(added: false, e.Index, e.ColumnCount));
	}

	private void UpdateDataAndNotify()
	{
		UpdateData();
		PostEvents();
	}

	private void UpdateData()
	{
		if (_columnTerrainMap.AnyColumnChanged)
		{
			int maxColumnCount = MaxColumnCount;
			MaxColumnCount = _columnTerrainMap.MaxColumnCount;
			if (maxColumnCount < MaxColumnCount)
			{
				Array.Resize(ref _terrainColumns, _verticalStride * MaxColumnCount);
			}
			_columnTerrainMap.CopyTerrainColumnsData(_terrainColumns, _columnCounts, Math.Max(maxColumnCount, MaxColumnCount));
			if (maxColumnCount != MaxColumnCount)
			{
				MaxTerrainColumnCountChanged?.Invoke(this, MaxColumnCount);
			}
		}
	}

	private void PostEvents()
	{
		ColumnChange result;
		while (_columnChanges.TryDequeue(out result))
		{
			int num = result.Index % _verticalStride;
			int num2 = result.Index / _verticalStride;
			int columnCount = result.ColumnCount;
			if (result.Added)
			{
				for (int num3 = columnCount - 1; num3 > num2; num3--)
				{
					ColumnMovedUp?.Invoke(this, num3 * _verticalStride + num);
				}
				ColumnReset?.Invoke(this, num2 * _verticalStride + num);
			}
			else
			{
				for (int i = num2; i < columnCount; i++)
				{
					ColumnMovedDown?.Invoke(this, i * _verticalStride + num);
				}
				ColumnReset?.Invoke(this, columnCount * _verticalStride + num);
			}
		}
	}
}

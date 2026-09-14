using System;
using Timberborn.Common;

namespace Timberborn.TerrainSystem;

public interface IThreadSafeColumnTerrainMap
{
	int MaxColumnCount { get; }

	ReadOnlyArray<byte> ColumnCounts { get; }

	ReadOnlyArray<ReadOnlyTerrainColumn> TerrainColumns { get; }

	event EventHandler<int> ColumnMovedUp;

	event EventHandler<int> ColumnMovedDown;

	event EventHandler<int> ColumnReset;

	event EventHandler<int> MaxTerrainColumnCountChanged;

	int GetColumnCount(int index);

	int GetColumnCeiling(int index3D);

	int GetColumnFloor(int index3D);

	bool TryGetIndexAtCeiling(int index2D, int ceiling, out int index3D);

	bool TryGetIndexAtOrAboveCeiling(int index2D, int ceiling, out int index3D);
}

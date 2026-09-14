using System;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.WaterSystem;

public interface IThreadSafeWaterMap
{
	int MaxColumnCount { get; }

	bool AnyColumnChanged { get; }

	ReadOnlyArray<byte> ColumnCounts { get; }

	ReadOnlyArray<ReadOnlyWaterColumn> WaterColumns { get; }

	ReadOnlyArray<Vector2> FlowDirections { get; }

	event EventHandler<int> MaxWaterColumnCountChanged;

	int ColumnCount(int index2D);

	byte ColumnFloor(int index3D);

	byte ColumnCeiling(int index3D);

	byte ColumnCeiling(Vector3Int coordinates);

	float WaterDepth(int index3D);

	float WaterDepth(Vector3Int coordinates);

	float ColumnContamination(Vector3Int coordinates);

	float ColumnOverflow(Vector3Int coordinates);

	Vector2 WaterFlowDirection(Vector3Int coordinates);

	bool TryGetColumnFloor(Vector3Int coordinates, out int floor);

	int CeiledWaterHeight(Vector3Int coordinates);

	float WaterHeightOrFloor(Vector3Int coordinates);

	bool CellIsUnderwater(Vector3Int coordinates);

	bool IsWaterOnAnyHeight(Vector2Int coordinates);
}

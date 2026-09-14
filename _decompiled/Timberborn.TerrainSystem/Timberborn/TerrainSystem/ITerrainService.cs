using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.TerrainSystem;

public interface ITerrainService
{
	Vector3Int Size { get; }

	int MaxTerrainHeight { get; }

	int MinTerrainHeight { get; }

	event EventHandler<TerrainHeightChangeEventArgs> PreTerrainHeightChanged;

	event EventHandler<TerrainHeightChangeEventArgs> TerrainHeightChanged;

	event EventHandler MinMaxTerrainHeightChanged;

	event EventHandler<Vector3Int> FieldOrCutoutChanged;

	int GetTerrainHeight(Vector3Int coordinates);

	bool TryGetRelativeHeight(Vector3Int coordinates, out int relativeHeight);

	int GetTerrainHeightBelow(Vector3Int coordinates);

	IEnumerable<Vector3Int> GetAllHeightsInCell(Vector2Int cellCoordinates);

	bool UnsafeCellIsTerrain(int index);

	bool CellIsField(Vector3Int cellCoordinates);

	bool CellIsCutout(Vector3Int cellCoordinates);

	bool Underground(Vector3Int coords);

	bool OnGround(Vector3Int coords);

	void SetTerrain(Vector3Int coordinates, int heightChange = 1);

	void UnsetTerrain(Vector3Int coordinates, int heightChange = 1);

	void UnsetTerrain(HashSet<Vector3Int> coordinates);

	void SetField(Vector3Int coordinates);

	void UnsetField(Vector3Int coordinates);

	void SetCutout(Vector3Int coordinates);

	void UnsetCutout(Vector3Int coordinates);

	bool Contains(Vector2Int coordinates);

	bool Contains(Vector3Int coordinates);

	Vector3Int Clamp(Vector3Int coordinates);

	int GetColumnCount(int index);

	int GetColumnFloor(int index3D);

	int GetColumnCeiling(int index3D);

	bool TryGetDistanceToTerrainAbove(Vector3Int coordinates, out int distance);

	bool IsVisible(Vector3Int coordinates);
}

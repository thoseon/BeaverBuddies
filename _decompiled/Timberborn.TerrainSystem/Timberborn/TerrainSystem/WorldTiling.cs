using UnityEngine;

namespace Timberborn.TerrainSystem;

public static class WorldTiling
{
	public static readonly int HorizontalTileSize = 16;

	private static readonly int VerticalTileSize = 8;

	public static Vector2Int TileCount2D(int xSize, int ySize)
	{
		return new Vector2Int(HorizontalTileCount(xSize), HorizontalTileCount(ySize));
	}

	public static Vector3Int TileCount3D(int xSize, int ySize, int zSize)
	{
		return new Vector3Int(HorizontalTileCount(xSize), HorizontalTileCount(ySize), VerticalTileCount(zSize));
	}

	public static Vector2Int CoordinatesToTileIndex2D(Vector2Int coordinates)
	{
		return CoordinatesToTileIndex2D(coordinates.x, coordinates.y);
	}

	public static Vector2Int CoordinatesToTileIndex2D(int x, int y)
	{
		return new Vector2Int(x / HorizontalTileSize, y / HorizontalTileSize);
	}

	public static Vector3Int CoordinatesToTileIndex3D(Vector3Int coordinates)
	{
		return new Vector3Int(coordinates.x / HorizontalTileSize, coordinates.y / HorizontalTileSize, coordinates.z / VerticalTileSize);
	}

	public static Vector3Int TileIndex3DToCoordinates(int index, int tileCountX, int tileCountY)
	{
		int x = index % tileCountX;
		int y = index / tileCountX % tileCountY;
		int z = index / (tileCountX * tileCountY);
		return new Vector3Int(x, y, z);
	}

	public static TileBounds2D TileBounds2D(Vector2Int tileIndex)
	{
		int x = tileIndex.x;
		int y = tileIndex.y;
		int num = x * HorizontalTileSize;
		int maxX = num + HorizontalTileSize;
		int num2 = y * HorizontalTileSize;
		int maxY = num2 + HorizontalTileSize;
		return new TileBounds2D(num, num2, maxX, maxY);
	}

	public static TileBounds3D TileBounds3D(Vector3Int tileIndex)
	{
		int x = tileIndex.x;
		int y = tileIndex.y;
		int z = tileIndex.z;
		int num = x * HorizontalTileSize;
		int maxX = num + HorizontalTileSize;
		int num2 = y * HorizontalTileSize;
		int maxY = num2 + HorizontalTileSize;
		int num3 = z * VerticalTileSize - 1;
		int maxZ = num3 + VerticalTileSize;
		return new TileBounds3D(num, num2, num3, maxX, maxY, maxZ);
	}

	private static int HorizontalTileCount(int size)
	{
		return (size + HorizontalTileSize - 1) / HorizontalTileSize;
	}

	private static int VerticalTileCount(int size)
	{
		return (size + VerticalTileSize - 1) / VerticalTileSize;
	}
}

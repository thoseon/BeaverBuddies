using System;
using System.Collections.Generic;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.Brushes;

public class BrushShapeIterator
{
	private readonly ITerrainService _terrainService;

	public BrushShapeIterator(ITerrainService terrainService)
	{
		_terrainService = terrainService;
	}

	public IEnumerable<Vector3Int> IterateShape(Vector3Int center, int size, BrushShape brushShape)
	{
		return brushShape switch
		{
			BrushShape.Square => IterateSquare(center, size), 
			BrushShape.Round => IterateRound(center, size), 
			_ => throw new ArgumentException(string.Format("Unexpected {0}: {1}", "BrushShape", brushShape)), 
		};
	}

	private IEnumerable<Vector3Int> IterateSquare(Vector3Int center, int size)
	{
		int num = size - 1;
		int minX = center.x - num;
		int maxX = center.x + num;
		int y = center.y - num;
		int maxY = center.y + num;
		Vector3Int coords = new Vector3Int(0, 0, center.z)
		{
			y = y
		};
		while (coords.y <= maxY)
		{
			coords.x = minX;
			int x;
			while (coords.x <= maxX)
			{
				if (_terrainService.Contains(coords))
				{
					yield return coords;
				}
				x = coords.x + 1;
				coords.x = x;
			}
			x = coords.y + 1;
			coords.y = x;
		}
	}

	private IEnumerable<Vector3Int> IterateRound(Vector3Int center, int size)
	{
		int num = size - 1;
		int minX = center.x - num;
		int maxX = center.x + num;
		int y = center.y - num;
		int maxY = center.y + num;
		Vector3Int coords = new Vector3Int(0, 0, center.z)
		{
			y = y
		};
		while (coords.y <= maxY)
		{
			coords.x = minX;
			int x;
			while (coords.x <= maxX)
			{
				if (_terrainService.Contains(coords) && Vector3.Distance(coords, center) + 0.7f <= (float)size)
				{
					yield return coords;
				}
				x = coords.x + 1;
				coords.x = x;
			}
			x = coords.y + 1;
			coords.y = x;
		}
	}
}

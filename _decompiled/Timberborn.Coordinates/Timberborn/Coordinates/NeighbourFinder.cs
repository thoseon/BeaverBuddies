using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Coordinates;

public static class NeighbourFinder
{
	public static IEnumerable<Vector2Int> GetSpiralNeighboursXY(int range)
	{
		int num = range * 2 + 1;
		int neighbours = num * num - 1;
		int x = 0;
		int y = 0;
		int dx = 0;
		int dy = -1;
		for (int i = 0; i < neighbours; i++)
		{
			if (x == y || (x < 0 && x == -y) || (x > 0 && x == 1 - y))
			{
				int num2 = -dy;
				dy = dx;
				dx = num2;
			}
			x += dx;
			y += dy;
			yield return new Vector2Int(x, y);
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.ZiplineSystem;

public class BresenhamLineDrawer
{
	public void DrawLine(Vector3Int start, Vector3Int end, ISet<Vector3Int> output)
	{
		Bresenham3D(start.x, start.y, start.z, end.x, end.y, end.z, output);
		Bresenham3D(end.x, end.y, end.z, start.x, start.y, start.z, output);
	}

	private static void Bresenham3D(int startX, int startY, int startZ, int endX, int endY, int endZ, ISet<Vector3Int> output)
	{
		int num = Math.Abs(endX - startX);
		int num2 = Math.Abs(endY - startY);
		int num3 = Math.Abs(endZ - startZ);
		int num4 = startX;
		int num5 = startY;
		int num6 = startZ;
		int num7 = ((endX > startX) ? 1 : (-1));
		int num8 = ((endY > startY) ? 1 : (-1));
		int num9 = ((endZ > startZ) ? 1 : (-1));
		int num10 = num - num2;
		int num11 = ((num > num2) ? num : num2);
		int num12 = num11 - num3;
		int num13 = num * 2;
		int num14 = num2 * 2;
		int num15 = num3 * 2;
		int num16 = num11 * 2;
		for (int num17 = 1 + num + num2 + num3; num17 > 0; num17--)
		{
			output.Add(new Vector3Int(num4, num5, num6));
			if (num12 <= 0)
			{
				num6 += num9;
				num12 += num16;
			}
			else if (num10 > 0)
			{
				num4 += num7;
				num10 -= num14;
				if (num >= num2)
				{
					num12 -= num15;
				}
			}
			else if (num10 < 0)
			{
				num5 += num8;
				num10 += num13;
				if (num2 > num)
				{
					num12 -= num15;
				}
			}
			else
			{
				num17--;
				if (num17 > 0)
				{
					output.Add(new Vector3Int(num4 + num7, num5, num6));
					output.Add(new Vector3Int(num4, num5 + num8, num6));
					num4 += num7;
					num5 += num8;
					num10 -= num14;
					num10 += num13;
					num12 -= num15;
				}
			}
		}
	}
}

using System;
using Timberborn.Common;

namespace Timberborn.TerrainSystem;

public class CeilingRetriever
{
	public int GetCeilingAtOrBelowHeight(in ReadOnlyArray<byte> columnCounts, in ReadOnlyArray<ReadOnlyTerrainColumn> columns, int verticalStride, int index2D, int height)
	{
		for (int num = columnCounts[index2D] - 1; num >= 0; num--)
		{
			int index = num * verticalStride + index2D;
			int ceiling = columns[index].Ceiling;
			if (ceiling <= height)
			{
				return ceiling;
			}
		}
		throw new InvalidOperationException();
	}
}

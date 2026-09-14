using Timberborn.Common;
using Timberborn.Multithreading;
using Timberborn.TerrainSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.WaterSystemRendering;

internal readonly struct UpdateWaterTexturesTask(ColumnChangeTracker columnChangeTracker, int stride, int verticalStride, Vector3Int mapSize, Vector2Int tileCount, DataTextureArray<float> waterDepths, DataTextureArray<Vector2> outflows, DataTextureArray<byte> contaminations, DataTextureArray<Vector2> columns, DataTextureArray<Vector2> linkBarriers, DataTextureArray<float> flowLimits, bool[] tilesWithWater, in ReadOnlyArray<byte> columnCounts, in ReadOnlyArray<ReadOnlyWaterColumn> waterColumns, in ReadOnlyArray<Vector2> flowDirections, in ReadOnlyArray<int> limitedDirections, in ReadOnlyJaggedArray<float> flowLimitsBuffer) : IParallelizerLoopTask
{
	private static readonly float MaxOutflow = 14f;

	private readonly ColumnChangeTracker _columnChangeTracker = columnChangeTracker;

	private readonly int _stride = stride;

	private readonly int _verticalStride = verticalStride;

	private readonly Vector3Int _mapSize = mapSize;

	private readonly Vector2Int _tileCount = tileCount;

	private readonly DataTextureArray<float> _waterDepths = waterDepths;

	private readonly DataTextureArray<Vector2> _outflows = outflows;

	private readonly DataTextureArray<byte> _contaminations = contaminations;

	private readonly DataTextureArray<Vector2> _columns = columns;

	private readonly DataTextureArray<Vector2> _linkBarriers = linkBarriers;

	private readonly DataTextureArray<float> _flowLimits = flowLimits;

	private readonly bool[] _tilesWithWater = tilesWithWater;

	private readonly ReadOnlyArray<byte> _columnCounts = columnCounts;

	private readonly ReadOnlyArray<ReadOnlyWaterColumn> _waterColumns = waterColumns;

	private readonly ReadOnlyArray<Vector2> _flowDirections = flowDirections;

	private readonly ReadOnlyArray<int> _limitedDirections = limitedDirections;

	private readonly ReadOnlyJaggedArray<float> _flowLimitsBuffer = flowLimitsBuffer;

	public void Run(int y)
	{
		bool flag = _columnChangeTracker.AnyColumnChanged();
		int num = (y + 1) * _stride;
		for (int i = 0; i < _mapSize.x; i++)
		{
			int num2 = num + i + 1;
			byte b = _columnCounts[num2];
			int num3 = i + y * _mapSize.x;
			for (int j = 0; j < b; j++)
			{
				int index = num2 + j * _verticalStride;
				ref readonly ReadOnlyWaterColumn reference = ref _waterColumns[index];
				float waterDepth = reference.WaterDepth;
				_waterDepths.NewData[j][num3] = waterDepth;
				byte b2 = (byte)(255f * reference.Contamination);
				_contaminations.NewData[j][num3] = b2;
				float num4 = _waterDepths.OldData[j][num3];
				if (waterDepth > 0f)
				{
					Vector2Int vector2Int = WorldTiling.CoordinatesToTileIndex2D(i, y);
					int num5 = vector2Int.x + vector2Int.y * _tileCount.x + j * _tileCount.x * _tileCount.y;
					_tilesWithWater[num5] = true;
					if (num4 <= 0f)
					{
						float num6 = waterDepth - 0.5f;
						if (num6 < 0f)
						{
							num6 = 0f;
						}
						_waterDepths.OldData[j][num3] = num6;
						_contaminations.OldData[j][num3] = b2;
					}
				}
				else if (num4 > 0f)
				{
					_contaminations.NewData[j][num3] = _contaminations.OldData[j][num3];
				}
				ref readonly Vector2 reference2 = ref _flowDirections[index];
				ref Vector2 reference3 = ref _outflows.NewData[j][num3];
				reference3.x = reference2.x / MaxOutflow;
				reference3.y = reference2.y / MaxOutflow;
				_flowLimits.NewData[j][num3] = _flowLimitsBuffer.Get(j, num3);
				if (flag)
				{
					ref Vector2 reference4 = ref _columns.NewData[j][num3];
					byte floor = reference.Floor;
					reference4.x = (int)floor;
					reference4.y = (int)reference.Ceiling;
					int num7 = num2 % _stride;
					int index2 = num2 / _stride * _stride + num7 + floor * _verticalStride;
					ref Vector2 reference5 = ref _linkBarriers.NewData[j][num3];
					reference5 = new Vector2((!CanOutflowTopOrBottom(_limitedDirections, index2)) ? 1 : 0, (!CanOutflowLeftOrRight(_limitedDirections, index2)) ? 1 : 0);
					Vector2 vector = _columns.OldData[j][num3];
					if (reference4.x != vector.x || reference4.y != vector.y)
					{
						_columns.OldData[j][num3] = reference4;
						_waterDepths.OldData[j][num3] = waterDepth;
						_linkBarriers.OldData[j][num3] = reference5;
					}
				}
			}
		}
	}

	private static bool CanOutflowLeftOrRight(ReadOnlyArray<int> limitDirection, int index)
	{
		int num = limitDirection[index];
		if (num != 0 && num != 1)
		{
			return num == -1;
		}
		return true;
	}

	private bool CanOutflowTopOrBottom(ReadOnlyArray<int> limitDirection, int index)
	{
		int num = limitDirection[index];
		if (num != 0 && num != _stride)
		{
			return num == -_stride;
		}
		return true;
	}
}

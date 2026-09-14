using System.Collections.Generic;
using Timberborn.MapIndexSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.WaterSystem;

internal class FlowVectorCalculator : ILoadableSingleton
{
	private readonly MapIndexService _mapIndexService;

	private int _verticalStride;

	public FlowVectorCalculator(MapIndexService mapIndexService)
	{
		_mapIndexService = mapIndexService;
	}

	public void Load()
	{
		_verticalStride = _mapIndexService.VerticalStride;
	}

	public Vector2 GetFlowVectorAtTop(in ColumnOutflows outflows)
	{
		if (outflows.Outflows == null)
		{
			return new Vector2(outflows.RightFlow.Flow - outflows.LeftFlow.Flow, outflows.TopFlow.Flow - outflows.BottomFlow.Flow);
		}
		float topFlow = GetTopFlow(in outflows.BottomFlow, _verticalStride, outflows.Outflows);
		float topFlow2 = GetTopFlow(in outflows.LeftFlow, _verticalStride, outflows.Outflows);
		float topFlow3 = GetTopFlow(in outflows.TopFlow, _verticalStride, outflows.Outflows);
		return new Vector2(GetTopFlow(in outflows.RightFlow, _verticalStride, outflows.Outflows) - topFlow2, topFlow3 - topFlow);
	}

	private static float GetTopFlow(in TargetedFlow inputFlow, int verticalStride, IList<TargetedFlow> outflows)
	{
		int index3D = inputFlow.Index3D;
		if (index3D == -1)
		{
			return 0f;
		}
		int num = index3D % verticalStride;
		int num2 = index3D;
		float flow = inputFlow.Flow;
		for (int i = 0; i < outflows.Count; i++)
		{
			TargetedFlow targetedFlow = outflows[i];
			int index3D2 = targetedFlow.Index3D;
			if (index3D2 % verticalStride == num && index3D2 > num2)
			{
				num2 = index3D2;
				flow = targetedFlow.Flow;
			}
		}
		return flow;
	}
}

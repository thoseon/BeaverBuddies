using System;
using System.Text;
using Timberborn.Common;
using Timberborn.MapIndexSystem;
using Timberborn.PackedListSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.WaterSystem;

internal class WaterMapLoader
{
	private class WaterFlowPackedListSerializer : PackedListSerializer<WaterFlow>
	{
		private static readonly char Separator = ':';

		protected override void Serialize(WaterFlow value, StringBuilder stringBuilder)
		{
			throw new NotSupportedException();
		}

		protected override WaterFlow Deserialize(string value)
		{
			string[] array = value.Split(Separator);
			return new WaterFlow
			{
				Bottom = float.Parse(array[0]),
				Left = float.Parse(array[1]),
				Top = float.Parse(array[2]),
				Right = float.Parse(array[3])
			};
		}
	}

	private struct WaterFlow
	{
		public float Bottom;

		public float Left;

		public float Top;

		public float Right;
	}

	private static readonly SingletonKey WaterMapKey = new SingletonKey("WaterMap");

	private static readonly PropertyKey<PackedList<float>> WaterDepthsKey = new PropertyKey<PackedList<float>>("WaterDepths");

	private static readonly PropertyKey<PackedList<WaterFlow>> OutflowsKey = new PropertyKey<PackedList<WaterFlow>>("Outflows");

	private static readonly SingletonKey ContaminationMapKey = new SingletonKey("ContaminationMap");

	private static readonly PropertyKey<PackedList<float>> ContaminationsKey = new PropertyKey<PackedList<float>>("Contaminations");

	private static readonly SingletonKey PollutionMapKey = new SingletonKey("PollutionMap");

	private static readonly PropertyKey<PackedList<float>> PollutionsKey = new PropertyKey<PackedList<float>>("Pollutions");

	private readonly MapIndexService _mapIndexService;

	private readonly FloatPackedListSerializer _floatPackedListSerializer;

	private readonly ISingletonLoader _singletonLoader;

	public WaterMapLoader(MapIndexService mapIndexService, FloatPackedListSerializer floatPackedListSerializer, ISingletonLoader singletonLoader)
	{
		_mapIndexService = mapIndexService;
		_floatPackedListSerializer = floatPackedListSerializer;
		_singletonLoader = singletonLoader;
	}

	[BackwardCompatible(2023, 11, 7, Compatibility.Map)]
	public void Load(Span<WaterColumn> waterColumns, Span<ColumnOutflows> outflows)
	{
		if (_singletonLoader.TryGetSingleton(WaterMapKey, out var objectLoader))
		{
			LoadWater(objectLoader, waterColumns, outflows);
			LoadContamination(waterColumns);
		}
	}

	private void LoadWater(IObjectLoader waterMap, Span<WaterColumn> waterColumns, Span<ColumnOutflows> outflows)
	{
		PackedList<float> packedList = waterMap.Get(WaterDepthsKey, _floatPackedListSerializer);
		float[] array = _mapIndexService.Unpack(packedList);
		PackedList<WaterFlow> packedList2 = waterMap.Get(OutflowsKey, new WaterFlowPackedListSerializer());
		WaterFlow[] array2 = _mapIndexService.Unpack(packedList2);
		for (int i = 0; i < _mapIndexService.MaxIndex; i++)
		{
			ref WaterColumn reference = ref waterColumns[i];
			reference.WaterDepth = array[i];
			reference.Overflow = 0f;
			ref ColumnOutflows reference2 = ref outflows[i];
			int stride = _mapIndexService.Stride;
			int index3D = i - stride;
			int index3D2 = i - 1;
			int index3D3 = i + stride;
			int index3D4 = i + 1;
			reference2.BottomFlow = new TargetedFlow(array2[i].Bottom, index3D);
			reference2.LeftFlow = new TargetedFlow(array2[i].Left, index3D2);
			reference2.TopFlow = new TargetedFlow(array2[i].Top, index3D3);
			reference2.RightFlow = new TargetedFlow(array2[i].Right, index3D4);
		}
	}

	private void LoadContamination(Span<WaterColumn> waterColumns)
	{
		if (_singletonLoader.TryGetSingleton(ContaminationMapKey, out var objectLoader))
		{
			LoadContamination(objectLoader.Get(ContaminationsKey, _floatPackedListSerializer), waterColumns);
		}
		else if (_singletonLoader.TryGetSingleton(PollutionMapKey, out objectLoader))
		{
			LoadContamination(objectLoader.Get(PollutionsKey, _floatPackedListSerializer), waterColumns);
		}
	}

	private void LoadContamination(PackedList<float> contaminationsPacked, Span<WaterColumn> waterColumns)
	{
		float[] array = _mapIndexService.Unpack(contaminationsPacked);
		for (int i = 0; i < _mapIndexService.MaxIndex; i++)
		{
			waterColumns[i].Contamination = array[i];
		}
	}
}

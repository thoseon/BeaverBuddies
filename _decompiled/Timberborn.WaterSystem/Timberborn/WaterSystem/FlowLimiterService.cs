using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.MapEditorTickSystem;
using Timberborn.MapIndexSystem;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.WaterSystem;

[MapEditorTickable]
internal class FlowLimiterService : IFlowLimiterService, ITickableSingleton, ILoadableSingleton, IPostLoadableSingleton
{
	private readonly struct Modification
	{
		public Vector3Int Coordinates { get; }

		public float HeightLimit { get; }

		public FlowDirection? FlowDirection { get; }

		public float? InflowLimit { get; }

		private Modification(Vector3Int coordinates, float? heightLimit, FlowDirection? flowDirection, float? inflowLimit)
		{
			Coordinates = coordinates;
			HeightLimit = heightLimit ?? float.MinValue;
			FlowDirection = flowDirection;
			InflowLimit = inflowLimit;
		}

		public static Modification CreateHeightLimitModification(Vector3Int coordinates, float heightLimit)
		{
			return new Modification(coordinates, heightLimit, null, null);
		}

		public static Modification CreateDirectionModification(Vector3Int coordinates, FlowDirection flowDirection)
		{
			return new Modification(coordinates, null, flowDirection, null);
		}

		public static Modification CreateInflowLimitModification(Vector3Int coordinates, float inflowLimit)
		{
			return new Modification(coordinates, null, null, inflowLimit);
		}
	}

	private readonly MapIndexService _mapIndexService;

	private readonly Queue<Modification> _modifications = new Queue<Modification>();

	private float[] _heightLimits;

	private int[] _limitedDirections;

	private float[] _inflowLimits;

	private int _stride;

	public ReadOnlyArray<int> LimitedDirections => new ReadOnlyArray<int>(_limitedDirections);

	public ReadOnlyArray<float> HeightLimits => new ReadOnlyArray<float>(_heightLimits);

	public ReadOnlyArray<float> InflowLimits => new ReadOnlyArray<float>(_inflowLimits);

	public event EventHandler<int> HeightLimitValueChanged;

	public FlowLimiterService(MapIndexService mapIndexService)
	{
		_mapIndexService = mapIndexService;
	}

	public void Load()
	{
		_stride = _mapIndexService.Stride;
		int num = _mapIndexService.TotalSize.z + 2;
		int num2 = _mapIndexService.VerticalStride * num;
		_heightLimits = new float[num2];
		_limitedDirections = new int[num2];
		_inflowLimits = new float[num2];
		for (int i = 0; i < num2; i++)
		{
			_heightLimits[i] = float.MinValue;
			_inflowLimits[i] = float.MaxValue;
		}
	}

	public void PostLoad()
	{
		ProcessModifications();
	}

	public void Tick()
	{
		ProcessModifications();
	}

	public void UpdateHeightLimit(Vector3Int coordinates, float heightLimit)
	{
		_modifications.Enqueue(Modification.CreateHeightLimitModification(coordinates, heightLimit));
	}

	public void RemoveHeightLimit(Vector3Int coordinates)
	{
		_modifications.Enqueue(Modification.CreateHeightLimitModification(coordinates, float.MinValue));
	}

	public void SetInflowLimit(Vector3Int coordinates, float inflowLimit)
	{
		_modifications.Enqueue(Modification.CreateInflowLimitModification(coordinates, inflowLimit));
	}

	public void RemoveInflowLimit(Vector3Int coordinates)
	{
		_modifications.Enqueue(Modification.CreateInflowLimitModification(coordinates, float.MaxValue));
	}

	public void AddDirectionLimiter(Vector3Int coordinates, FlowDirection flowDirection)
	{
		_modifications.Enqueue(Modification.CreateDirectionModification(coordinates, flowDirection));
	}

	public void RemoveDirectionLimiter(Vector3Int coordinates)
	{
		_modifications.Enqueue(Modification.CreateDirectionModification(coordinates, FlowDirection.Any));
	}

	private void ProcessModifications()
	{
		while (!_modifications.IsEmpty())
		{
			ProcessModification(_modifications.Dequeue());
		}
	}

	private void ProcessModification(in Modification modification)
	{
		int num = _mapIndexService.CoordinatesToIndex3D(modification.Coordinates);
		if (modification.FlowDirection.HasValue)
		{
			_limitedDirections[num] = FlowDirectionToIntDirection(modification.FlowDirection.Value);
			return;
		}
		if (modification.InflowLimit.HasValue)
		{
			_inflowLimits[num] = modification.InflowLimit.Value;
			return;
		}
		_heightLimits[num] = modification.HeightLimit;
		HeightLimitValueChanged?.Invoke(this, num);
	}

	private int FlowDirectionToIntDirection(FlowDirection flowDirection)
	{
		return flowDirection switch
		{
			FlowDirection.Any => 0, 
			FlowDirection.Bottom => -_stride, 
			FlowDirection.Left => -1, 
			FlowDirection.Top => _stride, 
			FlowDirection.Right => 1, 
			_ => throw new ArgumentOutOfRangeException("flowDirection", flowDirection, null), 
		};
	}
}

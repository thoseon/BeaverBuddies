using System.Collections.Immutable;
using UnityEngine;

namespace Timberborn.WaterSystem;

internal readonly struct ThreadSafeWaterSource
{
	private readonly IWaterSource _waterSource;

	public float CurrentStrength { get; }

	public float Contamination { get; }

	public ImmutableArray<Vector3Int> Coordinates => _waterSource.Coordinates;

	public ThreadSafeWaterSource(IWaterSource waterSource)
	{
		_waterSource = waterSource;
		CurrentStrength = waterSource.CurrentStrength;
		Contamination = waterSource.Contamination;
	}
}

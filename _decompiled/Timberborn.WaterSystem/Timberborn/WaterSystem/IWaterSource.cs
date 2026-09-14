using System.Collections.Immutable;
using UnityEngine;

namespace Timberborn.WaterSystem;

public interface IWaterSource
{
	ImmutableArray<Vector3Int> Coordinates { get; }

	float SpecifiedStrength { get; }

	float CurrentStrength { get; }

	float Contamination { get; }

	void SetSpecifiedStrength(float strength);
}

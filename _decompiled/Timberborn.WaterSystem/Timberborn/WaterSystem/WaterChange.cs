using UnityEngine;

namespace Timberborn.WaterSystem;

internal readonly struct WaterChange
{
	public Vector3Int Coordinates { get; }

	public float DepthChange { get; }

	public float ContaminationChange { get; }

	public WaterChange(Vector3Int coordinates, float depthChange, float contaminationChange)
	{
		Coordinates = coordinates;
		DepthChange = depthChange;
		ContaminationChange = contaminationChange;
	}
}

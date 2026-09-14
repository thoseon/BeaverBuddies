using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.GameDistrictsUI;

internal record DistrictConnectionLineRendererSpec : ComponentSpec
{
	[Serialize]
	public AssetRef<LineRenderer> LineRendererPrefab { get; init; }

	[Serialize]
	public float ArcAngle { get; init; }

	[Serialize]
	public int CurvePoints { get; init; }

	[Serialize]
	public float LineCutoff { get; init; }
}

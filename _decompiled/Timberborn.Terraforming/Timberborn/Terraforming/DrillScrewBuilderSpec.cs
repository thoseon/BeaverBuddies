using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.Terraforming;

internal record DrillScrewBuilderSpec : ComponentSpec
{
	[Serialize]
	public string ParentName { get; init; }

	[Serialize]
	public Vector3 AnchorPosition { get; init; }

	[Serialize]
	public float DrillRadius { get; init; }

	[Serialize]
	public string ScrewHeadPrefabPath { get; init; }

	[Serialize]
	public string ScrewAxisPrefabPath { get; init; }
}

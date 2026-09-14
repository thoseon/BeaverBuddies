using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.SelectionSystem;

internal record BoxColliderAdderSpec : ComponentSpec
{
	[Serialize]
	public string TargetName { get; init; }

	[Serialize]
	public Vector3 Center { get; init; }

	[Serialize]
	public Vector3 Size { get; init; }
}

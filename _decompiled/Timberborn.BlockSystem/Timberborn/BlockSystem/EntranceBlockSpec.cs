using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BlockSystem;

public record EntranceBlockSpec
{
	[Serialize]
	public bool HasEntrance { get; init; }

	[Serialize]
	public Vector3Int Coordinates { get; init; }
}

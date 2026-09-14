using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.PathSystem;

internal record DrivewayModelInstantiatorSpec : ComponentSpec
{
	[Serialize]
	public AssetRef<GameObject> NarrowLeftDrivewayPrefab { get; init; }

	[Serialize]
	public AssetRef<GameObject> NarrowCenterDrivewayPrefab { get; init; }

	[Serialize]
	public AssetRef<GameObject> NarrowRightDrivewayPrefab { get; init; }

	[Serialize]
	public AssetRef<GameObject> WideCenterDrivewayPrefab { get; init; }

	[Serialize]
	public AssetRef<GameObject> LongCenterDrivewayPrefab { get; init; }

	[Serialize]
	public AssetRef<GameObject> StraightPathDrivewayPrefab { get; init; }
}

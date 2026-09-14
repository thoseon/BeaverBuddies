using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.ModularShafts;

internal record ModularShaftPartsSpec : ComponentSpec
{
	[Serialize]
	public AssetRef<GameObject> ShaftBase { get; init; }

	[Serialize]
	public AssetRef<GameObject> ShaftLowerFrame { get; init; }

	[Serialize]
	public AssetRef<GameObject> ShaftSupport { get; init; }

	[Serialize]
	public AssetRef<GameObject> ShaftFrame { get; init; }

	[Serialize]
	public AssetRef<GameObject> GearSmall { get; init; }

	[Serialize]
	public AssetRef<GameObject> GearMedium { get; init; }

	[Serialize]
	public AssetRef<GameObject> GearLarge { get; init; }

	[Serialize]
	public AssetRef<GameObject> GearBottomBase { get; init; }

	[Serialize]
	public AssetRef<GameObject> GearBottomSmall { get; init; }

	[Serialize]
	public AssetRef<GameObject> GearBottomLarge { get; init; }

	[Serialize]
	public AssetRef<GameObject> GearTopSmall { get; init; }

	[Serialize]
	public AssetRef<GameObject> GearTopLarge { get; init; }

	[Serialize]
	public AssetRef<GameObject> GearInner { get; init; }

	[Serialize]
	public AssetRef<GameObject> GearInnerLong { get; init; }

	[Serialize]
	public AssetRef<GameObject> GearInnerOpposite { get; init; }

	[Serialize]
	public AssetRef<GameObject> GearInnerThrough { get; init; }

	[Serialize]
	public AssetRef<GameObject> AxleInnerLong { get; init; }

	[Serialize]
	public AssetRef<GameObject> AxleVertical { get; init; }

	[Serialize]
	public AssetRef<GameObject> AxleHorizontal { get; init; }
}

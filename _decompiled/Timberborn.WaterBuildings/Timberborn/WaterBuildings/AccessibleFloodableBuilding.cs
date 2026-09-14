using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.WaterObjects;
using UnityEngine;

namespace Timberborn.WaterBuildings;

internal class AccessibleFloodableBuilding : BaseComponent, IAwakableComponent, IWaterObjectSpecification
{
	public Vector3Int WaterCoordinates { get; private set; }

	public void Awake()
	{
		BlockObjectSpec component = GetComponent<BlockObjectSpec>();
		WaterCoordinates = (component.Entrance.HasEntrance ? new Vector3Int(component.Entrance.Coordinates.x, component.Entrance.Coordinates.y, component.Entrance.Coordinates.z - component.BaseZ) : Vector3Int.zero);
	}
}

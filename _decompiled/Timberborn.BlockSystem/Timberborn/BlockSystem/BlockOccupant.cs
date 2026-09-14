using Timberborn.BaseComponentSystem;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.BlockSystem;

public class BlockOccupant : BaseComponent, IRegisteredComponent
{
	public Vector3 GridCoordinates => CoordinateSystem.WorldToGrid(base.Transform.position);
}

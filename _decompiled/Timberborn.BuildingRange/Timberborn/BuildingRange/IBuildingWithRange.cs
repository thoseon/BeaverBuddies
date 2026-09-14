using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using UnityEngine;

namespace Timberborn.BuildingRange;

public interface IBuildingWithRange
{
	string RangeName { get; }

	IEnumerable<Vector3Int> GetBlocksInRange();

	IEnumerable<BaseComponent> GetObjectsInRange();
}

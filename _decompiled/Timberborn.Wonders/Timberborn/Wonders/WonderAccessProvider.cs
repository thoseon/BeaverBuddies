using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Buildings;
using Timberborn.BuildingsNavigation;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.Wonders;

internal class WonderAccessProvider : BaseComponent, IAwakableComponent, IConstructionSiteAccessProvider
{
	private BuildingAccessible _buildingAccessible;

	public void Awake()
	{
		_buildingAccessible = GetComponent<BuildingAccessible>();
	}

	public IEnumerable<Vector3> GetAccesses()
	{
		return Enumerables.One(_buildingAccessible.CalculateAccess());
	}
}

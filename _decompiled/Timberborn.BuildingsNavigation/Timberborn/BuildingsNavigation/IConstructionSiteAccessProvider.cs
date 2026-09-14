using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.BuildingsNavigation;

public interface IConstructionSiteAccessProvider
{
	IEnumerable<Vector3> GetAccesses();
}

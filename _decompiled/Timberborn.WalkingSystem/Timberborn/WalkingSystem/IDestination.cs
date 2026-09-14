using System.Collections.Generic;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.WalkingSystem;

public interface IDestination
{
	bool FindPath(Vector3 start, List<PathCorner> pathCorners, out float distance);
}

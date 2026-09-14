using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Navigation;

public interface IPathToAccessibleModifier
{
	void ModifyPath(Vector3 start, List<PathCorner> pathCorners, ref float distance);
}

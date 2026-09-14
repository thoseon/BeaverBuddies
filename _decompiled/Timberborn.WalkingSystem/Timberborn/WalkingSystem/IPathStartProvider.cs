using System.Collections.Generic;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.WalkingSystem;

public interface IPathStartProvider
{
	bool TryGetPathStart(IDestination destination, List<PathCorner> pathCorners, out Vector3 start);
}

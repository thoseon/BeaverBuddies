using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CharacterNavigation;
using Timberborn.Common;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.WalkingSystem;

internal class OccupiedAccessiblePathStart : BaseComponent, IAwakableComponent, IPathStartProvider
{
	private Navigator _navigator;

	public void Awake()
	{
		_navigator = GetComponent<Navigator>();
	}

	public bool TryGetPathStart(IDestination destination, List<PathCorner> pathCorners, out Vector3 start)
	{
		Accessible accessible = _navigator.OccupiedAccessible();
		if (accessible != null && (destination as AccessibleDestination)?.Accessible != accessible)
		{
			start = accessible.FindExitPath(base.Transform.position, pathCorners);
			if (!pathCorners.IsEmpty())
			{
				pathCorners.RemoveLast();
			}
			return true;
		}
		start = default(Vector3);
		return false;
	}
}

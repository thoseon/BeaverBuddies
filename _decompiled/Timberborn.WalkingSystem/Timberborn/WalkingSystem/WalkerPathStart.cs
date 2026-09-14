using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.WalkingSystem;

internal class WalkerPathStart : BaseComponent, IAwakableComponent
{
	private readonly List<IPathStartProvider> _pathStartProviders = new List<IPathStartProvider>();

	public void Awake()
	{
		GetComponents(_pathStartProviders);
	}

	public void GetPathStart(IDestination destination, List<PathCorner> pathCorners, out Vector3 start)
	{
		foreach (IPathStartProvider pathStartProvider in _pathStartProviders)
		{
			if (pathStartProvider.TryGetPathStart(destination, pathCorners, out start))
			{
				return;
			}
		}
		start = base.Transform.position;
	}
}

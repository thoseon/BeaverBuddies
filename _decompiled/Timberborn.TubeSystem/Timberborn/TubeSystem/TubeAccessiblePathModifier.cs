using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.TubeSystem;

internal class TubeAccessiblePathModifier : BaseComponent, IAwakableComponent, IPostInitializableEntity, IPathToAccessibleModifier
{
	private readonly INavigationService _navigationService;

	private Accessible _accessible;

	private BlockObject _blockObject;

	private readonly List<PathCorner> _pathCornerCache = new List<PathCorner>();

	public TubeAccessiblePathModifier(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void PostInitializeEntity()
	{
		_accessible = GetEnabledComponent<Accessible>();
	}

	public void ModifyPath(Vector3 start, List<PathCorner> pathCorners, ref float distance)
	{
		if ((bool)_accessible && _blockObject.IsUnfinished)
		{
			if (_navigationService.FindRoadSpillOrTerrainPathUnlimitedRange(start, _accessible.Accesses, _pathCornerCache, out var distance2) && distance2 < distance)
			{
				pathCorners.Clear();
				pathCorners.AddRange(_pathCornerCache);
				distance = distance2;
			}
			_pathCornerCache.Clear();
		}
	}
}

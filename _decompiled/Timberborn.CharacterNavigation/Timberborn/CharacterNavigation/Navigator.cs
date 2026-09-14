using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.CharacterNavigation;

public class Navigator : BaseComponent
{
	private readonly IBlockService _blockService;

	public Navigator(IBlockService blockService)
	{
		_blockService = blockService;
	}

	public Vector3 CurrentAccessOrPosition()
	{
		return OccupiedAccessible()?.UnblockedSingleAccess ?? base.Transform.position;
	}

	public Accessible OccupiedAccessible()
	{
		Vector3Int coordinates = NavigationCoordinateSystem.WorldToGridInt(base.Transform.position);
		BlockObject bottomObjectAt = _blockService.GetBottomObjectAt(coordinates);
		if ((bool)bottomObjectAt)
		{
			Accessible enabledComponent = bottomObjectAt.GetEnabledComponent<Accessible>();
			if ((bool)enabledComponent && enabledComponent.HasSingleAccess)
			{
				return enabledComponent;
			}
		}
		return null;
	}
}

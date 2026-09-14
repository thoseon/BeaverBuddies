using Timberborn.BlockSystem;
using Timberborn.BuildingsReachability;
using Timberborn.Localization;
using Timberborn.StartingLocationSystem;

namespace Timberborn.GameStartup;

internal class StartingBuildingPlacementValidator : IBlockObjectValidator
{
	private static readonly string EntranceBlockedLocKey = "Buildings.EntranceBlocked";

	private readonly ILoc _loc;

	private readonly GameInitializer _gameInitializer;

	public StartingBuildingPlacementValidator(ILoc loc, GameInitializer gameInitializer)
	{
		_loc = loc;
		_gameInitializer = gameInitializer;
	}

	public bool IsValid(BlockObject blockObject, out string errorMessage)
	{
		if (IsNotValid(blockObject))
		{
			errorMessage = _loc.T(EntranceBlockedLocKey);
			return false;
		}
		errorMessage = null;
		return true;
	}

	private bool IsNotValid(BlockObject blockObject)
	{
		if (!_gameInitializer.IsGameInitialized && blockObject.HasComponent<StartingLocationSpec>())
		{
			BlockableEntranceBuilding component = blockObject.GetComponent<BlockableEntranceBuilding>();
			if (component != null)
			{
				return component.IsEntranceInaccessible();
			}
		}
		return false;
	}
}

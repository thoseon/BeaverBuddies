using System;
using Timberborn.BlockObjectTools;
using Timberborn.Buildings;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.ScienceSystem;
using Timberborn.ToolSystem;

namespace Timberborn.BuildingTools;

internal class BuildingToolLocker : IToolLocker
{
	private static readonly string CantUnlockLocKey = "BuildingTools.CantUnlock";

	private static readonly string UnlockPromptLocKey = "BuildingTools.UnlockPrompt";

	private static readonly string InstantUnlockKey = "InstantUnlock";

	private readonly InputService _inputService;

	private readonly BuildingUnlockingService _buildingUnlockingService;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly ILoc _loc;

	public BuildingToolLocker(InputService inputService, BuildingUnlockingService buildingUnlockingService, DialogBoxShower dialogBoxShower, ILoc loc)
	{
		_inputService = inputService;
		_buildingUnlockingService = buildingUnlockingService;
		_dialogBoxShower = dialogBoxShower;
		_loc = loc;
	}

	public bool ShouldLock(ITool tool)
	{
		if (TryGetBuildingFromTool(tool, out var buildingSpec))
		{
			return !_buildingUnlockingService.Unlocked(buildingSpec);
		}
		return false;
	}

	public void TryToUnlock(ITool tool, Action successCallback, Action failCallback)
	{
		BuildingSpec buildingFromToolUnsafe = GetBuildingFromToolUnsafe(tool);
		if (_inputService.IsKeyHeld(InstantUnlockKey))
		{
			UnlockIgnoringScienceCost(buildingFromToolUnsafe, successCallback);
		}
		else if (_buildingUnlockingService.Unlockable(buildingFromToolUnsafe))
		{
			AskForUnlockingConfirmation(buildingFromToolUnsafe, successCallback, failCallback);
		}
		else
		{
			ShowInsufficientSciencePointsMessage(buildingFromToolUnsafe, failCallback);
		}
	}

	private static bool TryGetBuildingFromTool(ITool tool, out BuildingSpec buildingSpec)
	{
		if (tool is BlockObjectTool blockObjectTool)
		{
			BuildingSpec spec = blockObjectTool.Template.GetSpec<BuildingSpec>();
			if ((object)spec != null)
			{
				buildingSpec = spec;
				return true;
			}
		}
		buildingSpec = null;
		return false;
	}

	private static BuildingSpec GetBuildingFromToolUnsafe(ITool tool)
	{
		if (TryGetBuildingFromTool(tool, out var buildingSpec))
		{
			return buildingSpec;
		}
		throw new ArgumentException($"Tool {tool.GetType()} is not a BlockObjectTool with a Building component");
	}

	private void UnlockIgnoringScienceCost(BuildingSpec buildingSpec, Action successCallback)
	{
		_buildingUnlockingService.UnlockIgnoringCost(buildingSpec);
		successCallback();
	}

	private void AskForUnlockingConfirmation(BuildingSpec buildingSpec, Action successCallback, Action failCallback)
	{
		_dialogBoxShower.Create().SetMessage(GetMessage(buildingSpec, UnlockPromptLocKey)).SetConfirmButton(delegate
		{
			_buildingUnlockingService.Unlock(buildingSpec);
			successCallback();
		})
			.SetCancelButton(failCallback)
			.Show();
	}

	private void ShowInsufficientSciencePointsMessage(BuildingSpec buildingSpec, Action failCallback)
	{
		_dialogBoxShower.Create().SetMessage(GetMessage(buildingSpec, CantUnlockLocKey)).SetConfirmButton(failCallback)
			.Show();
	}

	private string GetMessage(BuildingSpec buildingSpec, string key)
	{
		string param = _loc.T(buildingSpec.GetSpec<LabeledEntitySpec>().DisplayNameLocKey);
		return _loc.T(key, param, buildingSpec.ScienceCost);
	}
}

using System;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.ToolSystem;

namespace Timberborn.PlantingUI;

internal class PlantableToolLocker : IToolLocker
{
	private static readonly string UnlockPromptLocKey = "Planting.UnlockPrompt";

	private static readonly string InstantUnlockKey = "InstantUnlock";

	private readonly ILoc _loc;

	private readonly InputService _inputService;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly UnlockedPlantableGroupsRegistry _unlockedPlantableGroupsRegistry;

	public PlantableToolLocker(ILoc loc, InputService inputService, DialogBoxShower dialogBoxShower, UnlockedPlantableGroupsRegistry unlockedPlantableGroupsRegistry)
	{
		_loc = loc;
		_inputService = inputService;
		_dialogBoxShower = dialogBoxShower;
		_unlockedPlantableGroupsRegistry = unlockedPlantableGroupsRegistry;
	}

	public bool ShouldLock(ITool tool)
	{
		if (IsPlantingTool(tool, out var plantingTool))
		{
			return _unlockedPlantableGroupsRegistry.IsLocked(plantingTool.PlantableSpec);
		}
		return false;
	}

	public void TryToUnlock(ITool tool, Action successCallback, Action failCallback)
	{
		PlantingTool plantingToolUnsafe = GetPlantingToolUnsafe(tool);
		if (_unlockedPlantableGroupsRegistry.IsLocked(plantingToolUnsafe.PlantableSpec) && !_inputService.IsKeyHeld(InstantUnlockKey))
		{
			ShowLockedMessage(plantingToolUnsafe, failCallback);
		}
		else
		{
			successCallback();
		}
	}

	private static PlantingTool GetPlantingToolUnsafe(ITool tool)
	{
		if (IsPlantingTool(tool, out var plantingTool))
		{
			return plantingTool;
		}
		throw new InvalidOperationException(string.Format("Tool {0} is not a {1}", tool, "PlantingTool"));
	}

	private static bool IsPlantingTool(ITool tool, out PlantingTool plantingTool)
	{
		plantingTool = tool as PlantingTool;
		return plantingTool != null;
	}

	private void ShowLockedMessage(PlantingTool plantingTool, Action failCallback)
	{
		string displayNameLocKey = plantingTool.PlantableSpec.GetSpec<LabeledEntitySpec>().DisplayNameLocKey;
		string message = _loc.T(UnlockPromptLocKey, plantingTool.BuildingName, _loc.T(displayNameLocKey));
		_dialogBoxShower.Create().SetMessage(message).SetConfirmButton(failCallback)
			.Show();
	}
}

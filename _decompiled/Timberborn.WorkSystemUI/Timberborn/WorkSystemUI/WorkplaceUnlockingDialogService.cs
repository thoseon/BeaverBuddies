using System;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.WorkSystem;

namespace Timberborn.WorkSystemUI;

public class WorkplaceUnlockingDialogService
{
	private static readonly string CantUnlockLocKey = "Work.WorkplaceUnlock.CantUnlock";

	private static readonly string UnlockPromptLocKey = "Work.WorkplaceUnlock.Prompt";

	private static readonly string InstantUnlockKey = "InstantUnlock";

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly InputService _inputService;

	private readonly WorkplaceUnlockingService _workplaceUnlockingService;

	private readonly ILoc _loc;

	public WorkplaceUnlockingDialogService(DialogBoxShower dialogBoxShower, InputService inputService, WorkplaceUnlockingService workplaceUnlockingService, ILoc loc)
	{
		_dialogBoxShower = dialogBoxShower;
		_inputService = inputService;
		_workplaceUnlockingService = workplaceUnlockingService;
		_loc = loc;
	}

	public void TryToUnlockWorkerType(UnlockableWorkerType unlockableWorkerType, Action callback)
	{
		if (!_workplaceUnlockingService.Unlocked(unlockableWorkerType))
		{
			if (_inputService.IsKeyHeld(InstantUnlockKey))
			{
				UnlockIgnoringScienceCost(unlockableWorkerType, callback);
			}
			else if (_workplaceUnlockingService.Unlockable(unlockableWorkerType))
			{
				AskForUnlockingConfirmation(unlockableWorkerType, callback);
			}
			else
			{
				ShowInsufficientSciencePointsMessage();
			}
		}
	}

	public bool IsWorkerTypeUnlocked(UnlockableWorkerType unlockableWorkerType)
	{
		return _workplaceUnlockingService.Unlocked(unlockableWorkerType);
	}

	public int GetWorkerTypeUnlockCost(UnlockableWorkerType unlockableWorkerType)
	{
		return _workplaceUnlockingService.GetUnlockCost(unlockableWorkerType);
	}

	private void UnlockIgnoringScienceCost(UnlockableWorkerType unlockableWorkerType, Action callback)
	{
		_workplaceUnlockingService.UnlockIgnoringCost(unlockableWorkerType);
		callback();
	}

	private void AskForUnlockingConfirmation(UnlockableWorkerType unlockableWorkerType, Action callback)
	{
		_dialogBoxShower.Create().SetMessage(GetUnlockPromptMessage(unlockableWorkerType)).SetConfirmButton(delegate
		{
			_workplaceUnlockingService.Unlock(unlockableWorkerType);
			callback();
		})
			.SetDefaultCancelButton()
			.Show();
	}

	private void ShowInsufficientSciencePointsMessage()
	{
		_dialogBoxShower.Create().SetLocalizedMessage(CantUnlockLocKey).Show();
	}

	private string GetUnlockPromptMessage(UnlockableWorkerType unlockableWorkerType)
	{
		return _loc.T(UnlockPromptLocKey, _workplaceUnlockingService.GetUnlockCost(unlockableWorkerType));
	}
}

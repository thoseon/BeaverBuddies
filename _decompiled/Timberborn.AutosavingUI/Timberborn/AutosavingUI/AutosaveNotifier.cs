using System;
using System.IO;
using Timberborn.Autosaving;
using Timberborn.GameSaveRuntimeSystem;
using Timberborn.Localization;
using Timberborn.QuickNotificationSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.AutosavingUI;

internal class AutosaveNotifier : ILoadableSingleton
{
	private static readonly string Success = "Autosave.Success";

	private static readonly string Failure = "Autosave.Failure";

	private static readonly string FailureDueToFullDisk = "Autosave.FailureDueToFullDisk";

	private readonly EventBus _eventBus;

	private readonly QuickNotificationService _quickNotificationService;

	private readonly ILoc _loc;

	public AutosaveNotifier(EventBus eventBus, QuickNotificationService quickNotificationService, ILoc loc)
	{
		_eventBus = eventBus;
		_quickNotificationService = quickNotificationService;
		_loc = loc;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnAutosave(AutosaveEvent autosaveEvent)
	{
		if (autosaveEvent.Successful)
		{
			NotifyAboutSuccess();
		}
		else
		{
			NotifyAboutFailure(autosaveEvent.Exception);
		}
	}

	private void NotifyAboutSuccess()
	{
		_quickNotificationService.SendNotification(_loc.T(Success));
	}

	private void NotifyAboutFailure(GameSaverException gameSaverException)
	{
		string text = (IsFullDisk(gameSaverException.InnerException) ? _loc.T(FailureDueToFullDisk) : _loc.T(Failure));
		_quickNotificationService.SendWarningNotification(text);
	}

	private static bool IsFullDisk(Exception exception)
	{
		if (exception is IOException ex)
		{
			if ((ex.HResult & 0xFFFF) != 39)
			{
				return (ex.HResult & 0xFFFF) == 112;
			}
			return true;
		}
		return false;
	}
}

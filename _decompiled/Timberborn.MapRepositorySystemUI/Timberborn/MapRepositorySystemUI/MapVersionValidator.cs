using System;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.MapRepositorySystem;
using Timberborn.Versioning;

namespace Timberborn.MapRepositorySystemUI;

internal class MapVersionValidator : IMapLoadValidator
{
	private static readonly string SemiCompatibleMapVersionLocKey = "MapSelection.SemiCompatibleMapVersion";

	private static readonly string NonCompatibleMapVersionLocKey = "MapSelection.NonCompatibleMapVersion";

	private readonly MapVersionCompatibilityService _mapVersionCompatibilityService;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly ILoc _loc;

	public int Priority => 0;

	public MapVersionValidator(MapVersionCompatibilityService mapVersionCompatibilityService, DialogBoxShower dialogBoxShower, ILoc loc)
	{
		_mapVersionCompatibilityService = mapVersionCompatibilityService;
		_dialogBoxShower = dialogBoxShower;
		_loc = loc;
	}

	public void ValidateForNewGame(MapFileReference mapFileReference, Action continueCallback)
	{
		ValidateMap(mapFileReference, continueCallback, acceptSemiCompatibility: false);
	}

	public void ValidateForMapEditor(MapFileReference mapFileReference, Action continueCallback)
	{
		ValidateMap(mapFileReference, continueCallback, acceptSemiCompatibility: true);
	}

	private void ValidateMap(MapFileReference mapFileReference, Action continueCallback, bool acceptSemiCompatibility)
	{
		Timberborn.Versioning.Version mapVersionNumber = _mapVersionCompatibilityService.GetMapVersionNumber(mapFileReference);
		if (_mapVersionCompatibilityService.VersionIsFullyCompatible(mapVersionNumber))
		{
			continueCallback();
		}
		else if (_mapVersionCompatibilityService.VersionIsSemiCompatible(mapVersionNumber))
		{
			if (acceptSemiCompatibility)
			{
				continueCallback();
			}
			else
			{
				ShowSemiCompatibleDialog(continueCallback, mapVersionNumber);
			}
		}
		else
		{
			ShowNonCompatibleDialog(mapVersionNumber);
		}
	}

	private void ShowSemiCompatibleDialog(Action continueCallback, Timberborn.Versioning.Version mapVersion)
	{
		string message = _loc.T(SemiCompatibleMapVersionLocKey, mapVersion, GameVersions.CurrentVersion);
		_dialogBoxShower.Create().SetMessage(message).SetConfirmButton(continueCallback)
			.SetDefaultCancelButton(_loc.T(CommonLocKeys.CancelKey))
			.Show();
	}

	private void ShowNonCompatibleDialog(Timberborn.Versioning.Version mapVersion)
	{
		string message = _loc.T(NonCompatibleMapVersionLocKey, mapVersion, GameVersions.CurrentVersion);
		_dialogBoxShower.Create().SetMessage(message).Show();
	}
}

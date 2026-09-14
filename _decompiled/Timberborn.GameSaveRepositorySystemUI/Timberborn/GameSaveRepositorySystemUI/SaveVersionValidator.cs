using System;
using Timberborn.CoreUI;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.Localization;
using Timberborn.StoreSystem;
using Timberborn.Versioning;
using Timberborn.VersioningSerialization;

namespace Timberborn.GameSaveRepositorySystemUI;

internal class SaveVersionValidator : IGameLoadValidator
{
	private static readonly string SemiCompatibleSaveVersionLocKey = "Saving.SemiCompatibleSaveVersion";

	private static readonly string NonCompatibleSaveVersionLocKey = "Saving.NonCompatibleSaveVersion";

	private readonly SaveVersionCompatibilityService _saveVersionCompatibilityService;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly ILoc _loc;

	private readonly GameSaveDeserializer _gameSaveDeserializer;

	private readonly VersionSerializer _versionSerializer;

	private readonly IStore _store;

	public int Priority => 1;

	public SaveVersionValidator(SaveVersionCompatibilityService saveVersionCompatibilityService, DialogBoxShower dialogBoxShower, ILoc loc, GameSaveDeserializer gameSaveDeserializer, VersionSerializer versionSerializer, IStore store)
	{
		_saveVersionCompatibilityService = saveVersionCompatibilityService;
		_dialogBoxShower = dialogBoxShower;
		_loc = loc;
		_gameSaveDeserializer = gameSaveDeserializer;
		_versionSerializer = versionSerializer;
		_store = store;
	}

	public void ValidateSave(SaveReference saveReference, Action continueCallback)
	{
		Timberborn.Versioning.Version saveVersionNumber = GetSaveVersionNumber(saveReference);
		if (_saveVersionCompatibilityService.VersionIsFullyCompatible(saveVersionNumber))
		{
			continueCallback();
		}
		else if (_saveVersionCompatibilityService.VersionIsSemiCompatible(saveVersionNumber))
		{
			ShowSemiCompatibleDialog(saveVersionNumber, continueCallback);
		}
		else
		{
			ShowNonCompatibleDialog();
		}
	}

	private Timberborn.Versioning.Version GetSaveVersionNumber(SaveReference saveReference)
	{
		return _gameSaveDeserializer.ReadFromSaveFileUnsafe(saveReference, _versionSerializer);
	}

	private void ShowSemiCompatibleDialog(Timberborn.Versioning.Version saveVersion, Action continueCallback)
	{
		string message = _loc.T(SemiCompatibleSaveVersionLocKey, saveVersion.NumericWithBranch, GameVersions.CurrentVersion.NumericWithBranch);
		if (_saveVersionCompatibilityService.VersionIsForwardCompatible(saveVersion))
		{
			message = AddCompatibilityMessage(message);
		}
		_dialogBoxShower.Create().SetMessage(message).SetConfirmButton(continueCallback)
			.SetDefaultCancelButton()
			.Show();
	}

	private void ShowNonCompatibleDialog()
	{
		_dialogBoxShower.Create().SetMessage(AddCompatibilityMessage(_loc.T(NonCompatibleSaveVersionLocKey))).Show();
	}

	private string AddCompatibilityMessage(string message)
	{
		return message + _store.GetCompatibilityMessage();
	}
}

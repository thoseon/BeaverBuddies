using System;
using System.IO;
using System.IO.Compression;
using Timberborn.CoreUI;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.Localization;

namespace Timberborn.GameSaveRepositorySystemUI;

internal class SaveFileValidator : IGameLoadValidator
{
	private static readonly string InvalidFileLocKey = "Saving.InvalidFile";

	private readonly GameSaveRepository _gameSaveRepository;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly ILoc _loc;

	public int Priority => 0;

	public SaveFileValidator(GameSaveRepository gameSaveRepository, DialogBoxShower dialogBoxShower, ILoc loc)
	{
		_gameSaveRepository = gameSaveRepository;
		_dialogBoxShower = dialogBoxShower;
		_loc = loc;
	}

	public void ValidateSave(SaveReference saveReference, Action continueCallback)
	{
		if (SaveIsValid(saveReference))
		{
			continueCallback();
			return;
		}
		string message = _loc.T(InvalidFileLocKey, saveReference.SettlementReference.SettlementName, saveReference.SaveName);
		_dialogBoxShower.Create().SetMessage(message).Show();
	}

	private bool SaveIsValid(SaveReference saveReference)
	{
		try
		{
			using Stream stream = _gameSaveRepository.OpenSaveWithoutLogging(saveReference);
			using ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
			return zipArchive.Entries.Count > 0;
		}
		catch (InvalidDataException)
		{
			return false;
		}
	}
}

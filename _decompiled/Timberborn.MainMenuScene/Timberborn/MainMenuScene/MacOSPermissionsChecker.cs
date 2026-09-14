using System;
using Timberborn.ApplicationLifetime;
using Timberborn.CoreUI;
using Timberborn.FileSystem;
using UnityEngine;

namespace Timberborn.MainMenuScene;

internal class MacOSPermissionsChecker
{
	private static readonly string MissingMacOSPermissionsLocKey = "Saving.MissingMacOSPermissionsLocKey";

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly IFileService _fileService;

	public MacOSPermissionsChecker(DialogBoxShower dialogBoxShower, IFileService fileService)
	{
		_dialogBoxShower = dialogBoxShower;
		_fileService = fileService;
	}

	public void CheckPermissions(Action onSuccessfulCheck)
	{
		if (_fileService.HasDocumentsPermissions)
		{
			onSuccessfulCheck();
			return;
		}
		Debug.Log("Missing access to Documents folder. Shutting down.");
		_dialogBoxShower.Create().SetLocalizedMessage(MissingMacOSPermissionsLocKey).SetConfirmButton(OpenDocumentsFolderSettings)
			.Show();
	}

	private static void OpenDocumentsFolderSettings()
	{
		Application.OpenURL("x-apple.systempreferences:com.apple.preference.security?Privacy_DocumentsFolder");
		GameQuitter.Quit();
	}
}

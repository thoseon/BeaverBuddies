using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Timberborn.Debugging;
using Timberborn.ExperimentalModeSystem;
using Timberborn.FileSystem;
using Timberborn.PlatformUtilities;
using UnityEngine;

namespace Timberborn.GameSaveRepositorySystem;

public class GameSaveRepository
{
	public static readonly string AutosaveNameSuffix = ".autosave";

	public static readonly string DevelopmentSettlementName = "Unity Editor Settlements";

	private static readonly string SaveExtension = ".timber";

	private static readonly string DefaultSavesDir = "Saves";

	private static readonly string ExperimentalSavesDir = "ExperimentalSaves";

	private readonly IFileService _fileService;

	private readonly FilenameValidator _filenameValidator;

	private readonly DevModeManager _devModeManager;

	private readonly ExperimentalMode _experimentalMode;

	public string DefaultSaveDirectory => SaveDirectories()[0];

	public GameSaveRepository(IFileService fileService, FilenameValidator filenameValidator, ExperimentalMode experimentalMode, DevModeManager devModeManager = null)
	{
		_fileService = fileService;
		_filenameValidator = filenameValidator;
		_experimentalMode = experimentalMode;
		_devModeManager = devModeManager;
	}

	public Stream CreateSaveSkippingNameValidation(SaveReference saveReference)
	{
		CreateDirectoryForSettlement(saveReference.SettlementReference);
		string fileName = SaveNameToFileName(saveReference);
		return _fileService.CreateFile(fileName);
	}

	public Stream CreateSave(SaveReference saveReference)
	{
		if (NameIsInvalid(saveReference.SettlementReference.SettlementName))
		{
			throw new ArgumentException(saveReference.SettlementReference.SettlementName + " contains an illegal character");
		}
		if (NameIsInvalid(saveReference.SaveName))
		{
			throw new ArgumentException(saveReference.SaveName + " contains an illegal character");
		}
		return CreateSaveSkippingNameValidation(saveReference);
	}

	public bool NameIsInvalid(string name)
	{
		return _filenameValidator.NameIsInvalid(name);
	}

	public Stream OpenSave(SaveReference saveReference)
	{
		return OpenSaveInternal(saveReference, logOpening: true);
	}

	public Stream OpenSaveWithoutLogging(SaveReference saveReference)
	{
		return OpenSaveInternal(saveReference, logOpening: false);
	}

	public IEnumerable<SaveReference> GetAllSaves()
	{
		return GetAllSettlements().SelectMany(GetSaves);
	}

	public IEnumerable<SaveReference> GetSaves(SettlementReference settlementReference)
	{
		CreateSaveDirectories();
		CreateDirectoryForSettlement(settlementReference);
		return from saveName in (from file in Directory.GetFiles(SettlementReferenceIntoDirectoryName(settlementReference))
				where Path.GetExtension(file) == SaveExtension
				orderby new FileInfo(file).LastWriteTime.ToUniversalTime() descending
				select file).Select(Path.GetFileNameWithoutExtension)
			select new SaveReference(saveName, settlementReference);
	}

	public IEnumerable<SettlementReference> GetAllSettlements()
	{
		CreateSaveDirectories();
		List<string> list = new List<string>();
		string[] array = SaveDirectories();
		foreach (string path in array)
		{
			list.AddRange(Directory.GetDirectories(path).Where(DirectoryExistsAndNotEmpty));
		}
		return list.OrderByDescending(GetMostRecentSaveTime).Select(DirectoryNameIntoSettlementReference);
	}

	public SaveReference GetMostRecentSave()
	{
		return GetAllSaves().FirstOrDefault();
	}

	public bool SaveExists(SaveReference saveReference)
	{
		if (saveReference != null)
		{
			return _fileService.FileExists(SaveNameToFileName(saveReference));
		}
		return false;
	}

	public DateTime GetSaveLastWriteTime(SaveReference saveReference)
	{
		string fileName = SaveNameToFileName(saveReference);
		return _fileService.GetFileInfo(fileName).LastWriteTime.ToUniversalTime();
	}

	public void DeleteSave(SaveReference saveReference)
	{
		string fileName = SaveNameToFileName(saveReference);
		_fileService.DeleteFile(fileName);
	}

	public bool DeleteSaveSafely(SaveReference saveReference)
	{
		if (SaveExists(saveReference))
		{
			string text = SaveNameToFileName(saveReference);
			try
			{
				_fileService.DeleteFile(text);
			}
			catch (Exception ex)
			{
				Debug.LogError("Failed to delete " + text + " due to " + ex.Message);
				return false;
			}
		}
		return true;
	}

	public void DeleteSettlement(SettlementReference settlementReference)
	{
		string directoryName = SettlementReferenceIntoDirectoryName(settlementReference);
		_fileService.DeleteDirectory(directoryName);
	}

	public DirectoryCreationResult CreateDirectoryForSettlement(string settlementName)
	{
		return CreateDirectoryForSettlement(new SettlementReference(settlementName, DefaultSaveDirectory));
	}

	public string SettlementReferenceIntoDirectoryName(SettlementReference settlementReference)
	{
		return _fileService.CombineIntoPath(settlementReference.SaveDirectory, settlementReference.SettlementName);
	}

	public string SaveNameToFileName(SaveReference saveReference)
	{
		string path = SettlementReferenceIntoDirectoryName(saveReference.SettlementReference);
		return _fileService.CombineIntoPath(path, saveReference.SaveName, SaveExtension);
	}

	public bool DevelopmentSettlementExists()
	{
		return (from settlement in GetAllSettlements()
			select settlement.SettlementName).Contains(DevelopmentSettlementName);
	}

	private DirectoryCreationResult CreateDirectoryForSettlement(SettlementReference settlementReference)
	{
		if (_filenameValidator.NameIsInvalid(settlementReference.SettlementName))
		{
			return DirectoryCreationResult.NameInvalid;
		}
		string directoryPath = SettlementReferenceIntoDirectoryName(settlementReference);
		return _fileService.CreateDirectoryIfValid(directoryPath);
	}

	private string[] SaveDirectories()
	{
		if (!Application.isEditor)
		{
			DevModeManager devModeManager = _devModeManager;
			if (devModeManager == null || !devModeManager.Enabled)
			{
				if (_experimentalMode.IsExperimental)
				{
					return new string[1] { Path.Combine(UserDataFolder.Folder, ExperimentalSavesDir) };
				}
				return new string[1] { Path.Combine(UserDataFolder.Folder, DefaultSavesDir) };
			}
		}
		if (!_experimentalMode.IsExperimental)
		{
			return new string[2]
			{
				Path.Combine(UserDataFolder.Folder, DefaultSavesDir),
				Path.Combine(UserDataFolder.Folder, ExperimentalSavesDir)
			};
		}
		return new string[2]
		{
			Path.Combine(UserDataFolder.Folder, ExperimentalSavesDir),
			Path.Combine(UserDataFolder.Folder, DefaultSavesDir)
		};
	}

	private Stream OpenSaveInternal(SaveReference saveReference, bool logOpening)
	{
		string text = SaveNameToFileName(saveReference);
		if (logOpening)
		{
			Debug.Log("Opening file: " + text);
		}
		return _fileService.OpenFile(text);
	}

	private bool DirectoryExistsAndNotEmpty(string directoryName)
	{
		return _fileService.DirectoryExistsAndNotEmpty(directoryName, SaveExtension);
	}

	private static DateTime GetMostRecentSaveTime(string directoryName)
	{
		return (from file in Directory.GetFiles(directoryName)
			where Path.GetExtension(file) == SaveExtension
			select file).Max((string file) => new FileInfo(file).LastWriteTime.ToUniversalTime());
	}

	private SettlementReference DirectoryNameIntoSettlementReference(string directoryName)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
		return new SettlementReference(directoryInfo.Name, directoryInfo.Parent?.FullName);
	}

	private void CreateSaveDirectories()
	{
		string[] array = SaveDirectories();
		foreach (string directoryName in array)
		{
			_fileService.CreateDirectory(directoryName);
		}
	}
}

using System.IO;
using Timberborn.FileSystem;
using Timberborn.PlatformUtilities;

namespace Timberborn.PlayerDataSystem;

public class PlayerDataFileService
{
	public static readonly string PlayerDataDirectory = Path.Combine(UserDataFolder.Folder, "PlayerData");

	public static readonly string PlayerDataFilePath = Path.Combine(PlayerDataDirectory, "player.data");

	private static readonly string PlayerDataBackupSuffix = "old";

	private readonly IFileService _fileService;

	private static string PlayerDataBackupFilePath => PlayerDataFilePath + "." + PlayerDataBackupSuffix;

	public PlayerDataFileService(IFileService fileService)
	{
		_fileService = fileService;
	}

	public void CopyFile(string suffix)
	{
		if (_fileService.HasDocumentsPermissions && _fileService.FileExists(PlayerDataFilePath))
		{
			string destinationFileName = PlayerDataFilePath + "." + suffix;
			_fileService.CopyFile(PlayerDataFilePath, destinationFileName);
		}
	}

	public void BackupFile()
	{
		if (_fileService.HasDocumentsPermissions && _fileService.FileExists(PlayerDataBackupFilePath))
		{
			_fileService.DeleteFile(PlayerDataBackupFilePath);
		}
		CopyFile(PlayerDataBackupSuffix);
	}

	public void RestoreFromBackup()
	{
		if (_fileService.HasDocumentsPermissions)
		{
			if (_fileService.FileExists(PlayerDataFilePath))
			{
				_fileService.DeleteFile(PlayerDataFilePath);
			}
			if (_fileService.FileExists(PlayerDataBackupFilePath))
			{
				_fileService.CopyFile(PlayerDataBackupFilePath, PlayerDataFilePath);
			}
		}
	}
}

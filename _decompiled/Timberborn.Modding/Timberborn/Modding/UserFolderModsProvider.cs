using System.Collections.Generic;
using System.IO;
using Timberborn.FileSystem;
using Timberborn.PlatformUtilities;
using Timberborn.Versioning;

namespace Timberborn.Modding;

public class UserFolderModsProvider : IModsProvider
{
	public static readonly string ModsDirectoryName = "Mods";

	private readonly IFileService _fileService;

	public UserFolderModsProvider(IFileService fileService)
	{
		_fileService = fileService;
	}

	public IEnumerable<ModDirectory> GetModDirectories()
	{
		if (_fileService.HasDocumentsPermissions)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(UserDataFolder.Folder, ModsDirectoryName));
			_fileService.CreateDirectory(directoryInfo.FullName);
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directory in directories)
			{
				yield return new ModDirectory(directory, isUserMod: true, "Local", GameVersions.CurrentVersion, isSubdirectory: false);
			}
		}
	}
}

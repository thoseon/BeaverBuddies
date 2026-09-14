using System;
using System.IO;

namespace Timberborn.PlatformUtilities;

public static class DocumentsPermissions
{
	public static bool HasPermissions()
	{
		try
		{
			if (ApplicationPlatform.IsMacOS())
			{
				Directory.CreateDirectory(UserDataFolder.Folder).GetDirectories();
			}
			return true;
		}
		catch (UnauthorizedAccessException)
		{
			return false;
		}
	}
}

using System;
using System.IO;

namespace Timberborn.PlatformUtilities;

public static class UserDataFolder
{
	public static readonly string Folder = (ApplicationPlatform.IsMacOS() ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Documents", "Timberborn") : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Timberborn"));
}

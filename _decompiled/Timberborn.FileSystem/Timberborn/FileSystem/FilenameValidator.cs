using System.Linq;
using Timberborn.Common;

namespace Timberborn.FileSystem;

public class FilenameValidator
{
	private static readonly string[] SystemIllegalCharacters = new string[9] { "<", ">", ":", "\"", "/", "\\", "|", "?", "*" };

	private static readonly string[] SystemIllegalNames = new string[24]
	{
		"AUX", "PRN", "NUL", "CON", "COM0", "COM1", "COM2", "COM3", "COM4", "COM5",
		"COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5",
		"LPT6", "LPT7", "LPT8", "LPT9"
	};

	private static readonly string[] GameIllegalCharacters = new string[13]
	{
		".", ";", "#", "~", "`", "^", "$", "!", "%", "&",
		"@", "+", "="
	};

	private static readonly string[] AllIllegalCharacters = SystemIllegalCharacters.Concat(GameIllegalCharacters).ToArray();

	public bool NameIsInvalid(string name)
	{
		if (!string.IsNullOrWhiteSpace(name) && !AllIllegalCharacters.FastAny(name.Contains))
		{
			return SystemIllegalNames.Contains(name.ToUpper());
		}
		return true;
	}
}

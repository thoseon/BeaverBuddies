using System.Text;

namespace Timberborn.Common;

public static class StringExtensions
{
	private static readonly StringBuilder StringBuilder = new StringBuilder();

	public static string ToPascalCase(this string inputString)
	{
		if (string.IsNullOrEmpty(inputString))
		{
			return inputString;
		}
		bool flag = true;
		for (int i = 0; i < inputString.Length; i++)
		{
			char c = inputString[i];
			if (char.IsWhiteSpace(c))
			{
				flag = true;
				continue;
			}
			if (flag)
			{
				c = char.ToUpper(c);
				flag = false;
			}
			else
			{
				c = char.ToLower(c);
			}
			StringBuilder.Append(c);
		}
		return StringBuilder.ToStringAndClear();
	}

	public static string SplitPascalCase(this string inputString)
	{
		for (int i = 0; i < inputString.Length; i++)
		{
			if (ShouldBeSplit(i, inputString))
			{
				StringBuilder.Append(' ');
			}
			StringBuilder.Append(inputString[i]);
		}
		return StringBuilder.ToStringAndClear();
	}

	private static bool ShouldBeSplit(int index, string inputString)
	{
		if (IsMiddleChar(index, inputString) && char.IsUpper(inputString[index]))
		{
			if (!char.IsLower(inputString[index + 1]))
			{
				return char.IsLower(inputString[index - 1]);
			}
			return true;
		}
		return false;
	}

	private static bool IsMiddleChar(int index, string inputString)
	{
		if (index > 0)
		{
			return index < inputString.Length - 1;
		}
		return false;
	}
}

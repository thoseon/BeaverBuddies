using System.Text;

namespace Timberborn.Common;

public static class StringBuilderExtensions
{
	public static string ToStringWithoutNewLineEnd(this StringBuilder stringBuilder)
	{
		return stringBuilder.ToString().TrimEnd('\r', '\n');
	}

	public static string ToStringWithoutNewLineEndAndClean(this StringBuilder stringBuilder)
	{
		string result = stringBuilder.ToStringWithoutNewLineEnd();
		stringBuilder.Clear();
		return result;
	}

	public static string ToStringAndClear(this StringBuilder stringBuilder)
	{
		string result = stringBuilder.ToString();
		stringBuilder.Clear();
		return result;
	}
}

using System;

namespace Timberborn.Common;

public static class EnumExtensions
{
	public static T Next<T>(this T sourceEnum) where T : Enum
	{
		T[] array = (T[])Enum.GetValues(sourceEnum.GetType());
		int num = Array.IndexOf(array, sourceEnum) + 1;
		if (num != array.Length)
		{
			return array[num];
		}
		return array[num - 1];
	}

	public static T Previous<T>(this T sourceEnum) where T : Enum
	{
		T[] array = (T[])Enum.GetValues(sourceEnum.GetType());
		int num = Array.IndexOf(array, sourceEnum);
		if (num != 0)
		{
			return array[num - 1];
		}
		return array[num];
	}
}

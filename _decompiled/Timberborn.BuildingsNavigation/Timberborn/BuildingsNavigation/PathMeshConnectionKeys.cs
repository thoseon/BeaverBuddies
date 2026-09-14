using System;

namespace Timberborn.BuildingsNavigation;

internal static class PathMeshConnectionKeys
{
	public static readonly byte Nothing = 0;

	public static readonly byte Path = 1;

	public static readonly byte Building = 3;

	private static readonly byte AlternativePath = 2;

	private static readonly byte AlternativeBuilding = 4;

	public static byte ParseCharToByteKey(char modelNameChar)
	{
		return modelNameChar switch
		{
			'0' => Nothing, 
			'P' => Path, 
			'p' => AlternativePath, 
			'B' => Building, 
			'b' => AlternativeBuilding, 
			_ => throw new ArgumentOutOfRangeException($"{modelNameChar} isn't acceptable value"), 
		};
	}

	public static byte ToAlternativeKey(byte key)
	{
		if (key == Path)
		{
			return AlternativePath;
		}
		if (key == Building)
		{
			return AlternativeBuilding;
		}
		throw new ArgumentOutOfRangeException($"Can't find alternative value for {key}");
	}
}

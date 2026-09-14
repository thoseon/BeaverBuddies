using System;
using System.IO;

namespace Timberborn.AssetSystem;

public static class AssetPathHelper
{
	private static readonly string AssetPathPrefix = "assets/";

	private static readonly string ResourcesDirectory = "resources/";

	public static string NormalizeAssetPath(string assetPath)
	{
		return NormalizeAssetPath(assetPath, ResourcesDirectory);
	}

	public static string NormalizeAssetPath(string assetPath, string rootDirectory)
	{
		return NormalizePath(GetRelativePath(assetPath, rootDirectory));
	}

	public static string GetRelativePath(string assetPath)
	{
		return GetRelativePath(assetPath, ResourcesDirectory);
	}

	public static string NormalizePath(string path)
	{
		return path.Replace('\\', '/').ToLower();
	}

	private static string GetRelativePath(string assetPath, string rootDirectory)
	{
		assetPath = TruncatePath(assetPath, rootDirectory);
		string extension = Path.GetExtension(assetPath);
		string text;
		if (extension == null)
		{
			text = assetPath;
		}
		else
		{
			string text2 = assetPath;
			int length = extension.Length;
			text = text2.Substring(0, text2.Length - length);
		}
		assetPath = text;
		if (assetPath.StartsWith("/") || assetPath.StartsWith("\\"))
		{
			string text2 = assetPath;
			assetPath = text2.Substring(1, text2.Length - 1);
		}
		return assetPath;
	}

	private static string TruncatePath(string assetPath, string rootDirectory)
	{
		if (assetPath.StartsWith(rootDirectory))
		{
			string text = assetPath;
			int length = rootDirectory.Length;
			assetPath = text.Substring(length, text.Length - length);
		}
		else
		{
			string text2 = "/" + rootDirectory;
			int num = assetPath.IndexOf(text2, StringComparison.OrdinalIgnoreCase);
			if (num != -1)
			{
				string text = assetPath;
				int length = num + text2.Length;
				assetPath = text.Substring(length, text.Length - length);
			}
			else if (assetPath.StartsWith(AssetPathPrefix, StringComparison.OrdinalIgnoreCase))
			{
				string text = assetPath;
				int length = AssetPathPrefix.Length;
				assetPath = text.Substring(length, text.Length - length);
			}
		}
		return assetPath;
	}
}

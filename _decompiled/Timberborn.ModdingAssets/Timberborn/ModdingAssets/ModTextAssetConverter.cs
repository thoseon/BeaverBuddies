using System.Collections.Generic;
using System.IO;
using Timberborn.BlueprintSystem;
using Timberborn.SerializationSystem;
using UnityEngine;

namespace Timberborn.ModdingAssets;

internal class ModTextAssetConverter : IModFileConverter<TextAsset>
{
	private static readonly List<string> ValidExtensions = new List<string> { ".txt", ".json", ".xml", ".csv" };

	private readonly List<TextAsset> _assets = new List<TextAsset>();

	public bool CanConvert(FileInfo fileInfo)
	{
		if (!fileInfo.Name.EndsWith(BlueprintAsset.FullExtension))
		{
			return ValidExtensions.Contains(fileInfo.Extension);
		}
		return false;
	}

	public bool TryConvert(OrderedFile orderedFile, string path, SerializedObject metadata, out TextAsset asset)
	{
		FileInfo file = orderedFile.File;
		asset = new TextAsset(File.ReadAllText(file.FullName))
		{
			name = Path.GetFileNameWithoutExtension(file.FullName)
		};
		_assets.Add(asset);
		return true;
	}

	public void Reset()
	{
		foreach (TextAsset asset in _assets)
		{
			Object.Destroy(asset);
		}
		_assets.Clear();
	}
}

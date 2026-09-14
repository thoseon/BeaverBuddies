using System.Collections.Generic;
using System.IO;
using Timberborn.BlueprintSystem;
using Timberborn.SerializationSystem;
using UnityEngine;

namespace Timberborn.ModdingAssets;

internal class ModBlueprintConverter : IModFileConverter<BlueprintAsset>
{
	private readonly List<BlueprintAsset> _blueprints = new List<BlueprintAsset>();

	public bool CanConvert(FileInfo fileInfo)
	{
		return fileInfo.Name.EndsWith(BlueprintAsset.FullExtension);
	}

	public bool TryConvert(OrderedFile orderedFile, string path, SerializedObject metadata, out BlueprintAsset asset)
	{
		FileInfo file = orderedFile.File;
		asset = BlueprintAsset.Create(path, File.ReadAllText(file.FullName), orderedFile.Source);
		asset.name = Path.GetFileNameWithoutExtension(file.Name);
		_blueprints.Add(asset);
		return true;
	}

	public void Reset()
	{
		foreach (BlueprintAsset blueprint in _blueprints)
		{
			if (blueprint != null)
			{
				Object.Destroy(blueprint);
			}
		}
		_blueprints.Clear();
	}
}

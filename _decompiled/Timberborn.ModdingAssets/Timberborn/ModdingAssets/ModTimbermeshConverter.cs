using System;
using System.IO;
using Timberborn.AssetSystem;
using Timberborn.SerializationSystem;
using UnityEngine;

namespace Timberborn.ModdingAssets;

internal class ModTimbermeshConverter : IModFileConverter<BinaryData>
{
	private static readonly string BinaryDataCacheName = "BinaryDataCache";

	private static readonly string ValidExtension = ".timbermesh";

	private Lazy<GameObject> _binaryDataCache = new Lazy<GameObject>(CreateBinaryDataCache);

	public bool CanConvert(FileInfo fileInfo)
	{
		return fileInfo.Extension == ValidExtension;
	}

	public bool TryConvert(OrderedFile orderedFile, string path, SerializedObject metadata, out BinaryData asset)
	{
		FileInfo file = orderedFile.File;
		GameObject gameObject = new GameObject(Path.GetFileNameWithoutExtension(file.Name));
		gameObject.transform.SetParent(_binaryDataCache.Value.transform);
		asset = gameObject.AddComponent<BinaryData>();
		asset.SetData(File.ReadAllBytes(file.FullName));
		return true;
	}

	public void Reset()
	{
		_binaryDataCache = new Lazy<GameObject>(CreateBinaryDataCache);
	}

	private static GameObject CreateBinaryDataCache()
	{
		GameObject gameObject = new GameObject(BinaryDataCacheName);
		gameObject.SetActive(value: false);
		return gameObject;
	}
}

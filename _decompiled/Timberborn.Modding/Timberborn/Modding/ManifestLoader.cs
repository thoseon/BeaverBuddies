using System;
using System.Collections.Generic;
using System.IO;
using Timberborn.SerializationSystem;
using Timberborn.Versioning;
using UnityEngine;

namespace Timberborn.Modding;

public class ManifestLoader
{
	public static readonly string ManifestFileName = "manifest.json";

	private readonly SerializedObjectReaderWriter _serializedObjectReaderWriter;

	public ManifestLoader(SerializedObjectReaderWriter serializedObjectReaderWriter)
	{
		_serializedObjectReaderWriter = serializedObjectReaderWriter;
	}

	public bool TryLoadManifest(string modDirectory, out ModManifest manifest)
	{
		FileInfo fileInfo = new FileInfo(Path.Combine(modDirectory, ManifestFileName));
		if (fileInfo.Exists)
		{
			return TryDeserializeManifest(fileInfo, out manifest);
		}
		manifest = null;
		return false;
	}

	private bool TryDeserializeManifest(FileInfo manifestFile, out ModManifest manifest)
	{
		try
		{
			using FileStream stream = manifestFile.OpenRead();
			SerializedObject serializedObject = _serializedObjectReaderWriter.ReadJson(stream);
			manifest = new ModManifest(serializedObject.Get<string>("Name"), serializedObject.Has("Description") ? serializedObject.Get<string>("Description") : string.Empty, Timberborn.Versioning.Version.Create(serializedObject.Get<string>("Version")), serializedObject.Get<string>("Id"), Timberborn.Versioning.Version.Create(serializedObject.Get<string>("MinimumGameVersion")), GetVersionedMods(serializedObject, "RequiredMods"), GetVersionedMods(serializedObject, "OptionalMods"));
			return true;
		}
		catch (Exception arg)
		{
			Debug.LogWarning($"Failed to load mod manifest from {manifestFile.FullName}: {arg}");
			manifest = null;
			return false;
		}
	}

	private static IEnumerable<VersionedMod> GetVersionedMods(SerializedObject save, string arrayName)
	{
		if (save.Has(arrayName))
		{
			SerializedObject[] array = save.GetArray<SerializedObject>(arrayName);
			foreach (SerializedObject serializedObject in array)
			{
				string id = serializedObject.Get<string>("Id");
				string version = (serializedObject.Has("MinimumVersion") ? serializedObject.Get<string>("MinimumVersion") : "0");
				yield return new VersionedMod(id, Timberborn.Versioning.Version.Create(version));
			}
		}
	}
}

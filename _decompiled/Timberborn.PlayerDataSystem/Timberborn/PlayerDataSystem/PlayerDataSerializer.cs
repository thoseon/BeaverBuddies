using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;
using Timberborn.Common;
using Timberborn.FileSystem;
using UnityEngine;

namespace Timberborn.PlayerDataSystem;

internal class PlayerDataSerializer
{
	private static readonly string PlayerDataEntryName = "data.json";

	private readonly IFileService _fileService;

	public PlayerDataSerializer(IFileService fileService)
	{
		_fileService = fileService;
	}

	public void SaveData(Dictionary<string, string> data)
	{
		try
		{
			if (_fileService.HasDocumentsPermissions)
			{
				_fileService.CreateDirectory(PlayerDataFileService.PlayerDataDirectory);
				using MemoryStream memoryStream = new MemoryStream();
				SaveToStream(data, memoryStream);
				SaveToFile(memoryStream);
				return;
			}
		}
		catch (Exception ex) when (((ex is IOException || ex is ArgumentException || ex is JsonException) ? 1 : 0) != 0)
		{
			throw new InvalidOperationException("Failed saving player data.", ex);
		}
	}

	public Dictionary<string, string> LoadData(out bool success)
	{
		success = _fileService.HasDocumentsPermissions;
		if (_fileService.HasDocumentsPermissions && _fileService.FileExists(PlayerDataFileService.PlayerDataFilePath))
		{
			try
			{
				using Stream stream = _fileService.OpenFile(PlayerDataFileService.PlayerDataFilePath);
				return LoadFromStream(stream);
			}
			catch (Exception ex) when (((ex is IOException || ex is ArgumentException || ex is JsonException || ex is InvalidDataException) ? 1 : 0) != 0)
			{
				Debug.LogWarning($"Failed loading player data. Details: {ex}");
				success = false;
			}
		}
		return new Dictionary<string, string>();
	}

	private static void SaveToStream(Dictionary<string, string> data, MemoryStream memoryStream)
	{
		string value = JsonConvert.SerializeObject(data, Formatting.Indented);
		using ZipArchive zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true);
		using Stream stream = zipArchive.CreateEntry(PlayerDataEntryName, System.IO.Compression.CompressionLevel.Fastest).Open();
		using StreamWriter streamWriter = new StreamWriter(stream);
		streamWriter.Write(value);
	}

	private void SaveToFile(MemoryStream memoryStream)
	{
		using Stream destination = _fileService.CreateFile(PlayerDataFileService.PlayerDataFilePath);
		memoryStream.Position = 0L;
		memoryStream.CopyTo(destination);
	}

	private static Dictionary<string, string> LoadFromStream(Stream stream)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		using ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
		ZipArchiveEntry zipArchiveEntry = zipArchive.Entries.FirstOrDefault((ZipArchiveEntry entry) => entry.Name == PlayerDataEntryName);
		if (zipArchiveEntry != null)
		{
			using Stream stream2 = zipArchiveEntry.Open();
			using StreamReader streamReader = new StreamReader(stream2);
			string value = streamReader.ReadToEnd();
			dictionary.AddRange(JsonConvert.DeserializeObject<Dictionary<string, string>>(value));
		}
		return dictionary;
	}
}

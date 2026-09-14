using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;

namespace Timberborn.SaveSystem;

public class SaveReader
{
	public T ReadFromSaveStream<T>(Stream saveStream, ISaveEntryReader<T> saveEntryReader)
	{
		try
		{
			return ReadFromSaveStreamUnsafe(saveStream, saveEntryReader);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Failed to read save entry " + saveEntryReader.EntryName + ": " + ex);
			return default(T);
		}
	}

	public T ReadFromSaveStreamUnsafe<T>(Stream saveStream, ISaveEntryReader<T> saveEntryReader)
	{
		using ZipArchive zipArchive = new ZipArchive(saveStream, ZipArchiveMode.Read);
		ZipArchiveEntry zipArchiveEntry = zipArchive.Entries.FirstOrDefault((ZipArchiveEntry entry) => entry.Name == saveEntryReader.EntryName);
		if (zipArchiveEntry != null)
		{
			using (Stream entryStream = zipArchiveEntry.Open())
			{
				return saveEntryReader.ReadFromSaveEntryStream(entryStream);
			}
		}
		return BackwardCompatibleRead(saveStream, saveEntryReader);
	}

	private static T BackwardCompatibleRead<T>(Stream saveStream, ISaveEntryReader<T> saveEntryReader)
	{
		if (saveEntryReader is IBackwardCompatibleSaveEntryReader<T> backwardCompatibleSaveEntryReader)
		{
			return backwardCompatibleSaveEntryReader.BackwardCompatibleRead(saveStream);
		}
		return default(T);
	}
}

using System;
using System.IO;
using Timberborn.Persistence;
using Timberborn.SaveSystem;
using Timberborn.SerializationSystem;

namespace Timberborn.SaveMetadataSystem;

public class SaveMetadataSerializer : ISaveEntryReader<SaveMetadata>
{
	private static readonly PropertyKey<DateTime> TimestampKey = new PropertyKey<DateTime>("Timestamp");

	private static readonly PropertyKey<int> CycleKey = new PropertyKey<int>("Cycle");

	private static readonly PropertyKey<int> DayKey = new PropertyKey<int>("Day");

	private static readonly ListKey<ModReference> ModsKey = new ListKey<ModReference>("Mods");

	private readonly SerializedObjectReaderWriter _serializedObjectReaderWriter;

	private readonly ModReferenceSerializer _modReferenceSerializer;

	private readonly InvariantDateTimeSerializer _invariantDateTimeSerializer;

	public string EntryName => "save_metadata.json";

	public SaveMetadataSerializer(SerializedObjectReaderWriter serializedObjectReaderWriter, ModReferenceSerializer modReferenceSerializer, InvariantDateTimeSerializer invariantDateTimeSerializer)
	{
		_serializedObjectReaderWriter = serializedObjectReaderWriter;
		_modReferenceSerializer = modReferenceSerializer;
		_invariantDateTimeSerializer = invariantDateTimeSerializer;
	}

	public void WriteToSaveEntryStream(Stream entryStream, SaveMetadata saveMetadata)
	{
		_serializedObjectReaderWriter.WriteJson(GetSaveMetadataSerializedObject(saveMetadata), entryStream);
	}

	public SaveMetadata ReadFromSaveEntryStream(Stream entryStream)
	{
		return Deserialize(_serializedObjectReaderWriter.ReadJson(entryStream));
	}

	private SerializedObject GetSaveMetadataSerializedObject(SaveMetadata saveMetadata)
	{
		SerializedObject serializedObject = new SerializedObject();
		SaveMetadata(saveMetadata, new ObjectSaver(serializedObject));
		return serializedObject;
	}

	private SaveMetadata Deserialize(SerializedObject serializedObject)
	{
		return LoadMetadata(new ObjectLoader(serializedObject));
	}

	private void SaveMetadata(SaveMetadata saveMetadata, IObjectSaver objectSaver)
	{
		objectSaver.Set(TimestampKey, saveMetadata.Timestamp, _invariantDateTimeSerializer);
		objectSaver.Set(CycleKey, saveMetadata.Cycle);
		objectSaver.Set(DayKey, saveMetadata.Day);
		objectSaver.Set(ModsKey, saveMetadata.Mods, _modReferenceSerializer);
	}

	private SaveMetadata LoadMetadata(IObjectLoader objectLoader)
	{
		return new SaveMetadata(objectLoader.Get(TimestampKey, _invariantDateTimeSerializer), objectLoader.Get(CycleKey), objectLoader.Get(DayKey), LoadMods(objectLoader));
	}

	private ModReference[] LoadMods(IObjectLoader objectLoader)
	{
		if (!objectLoader.Has(ModsKey))
		{
			return Array.Empty<ModReference>();
		}
		return objectLoader.Get(ModsKey, _modReferenceSerializer).ToArray();
	}
}

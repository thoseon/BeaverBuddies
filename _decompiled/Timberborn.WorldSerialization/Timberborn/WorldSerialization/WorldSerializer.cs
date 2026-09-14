using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Timberborn.SaveSystem;
using Timberborn.SerializationSystem;
using Timberborn.Versioning;

namespace Timberborn.WorldSerialization;

public class WorldSerializer : ISaveEntryReader<SerializedWorld>
{
	private readonly SerializedObjectReaderWriter _serializedObjectReaderWriter;

	public string EntryName => "world.json";

	public WorldSerializer(SerializedObjectReaderWriter serializedObjectReaderWriter)
	{
		_serializedObjectReaderWriter = serializedObjectReaderWriter;
	}

	public void WriteToSaveEntryStream(Stream entryStream, SerializedWorld serializedWorld)
	{
		_serializedObjectReaderWriter.WriteJson(SerializeSave(serializedWorld), entryStream);
	}

	public SerializedWorld ReadFromSaveEntryStream(Stream entryStream)
	{
		return DeserializeSave(_serializedObjectReaderWriter.ReadJson(entryStream));
	}

	private static SerializedObject SerializeSave(SerializedWorld serializedWorld)
	{
		SerializedObject value = SerializedSingletons(serializedWorld);
		IEnumerable<SerializedObject> source = SerializedEntities(serializedWorld);
		SerializedObject serializedObject = new SerializedObject();
		serializedObject.Set("GameVersion", serializedWorld.Version.Full);
		serializedObject.Set("Timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
		serializedObject.Set("Singletons", value);
		serializedObject.SetArray("Entities", source.ToArray());
		return serializedObject;
	}

	private static SerializedObject SerializedSingletons(SerializedWorld serializedWorld)
	{
		SerializedObject serializedObject = new SerializedObject();
		foreach (SerializedSingleton item in serializedWorld.Singletons())
		{
			serializedObject.Set(item.Name, item.Value);
		}
		return serializedObject;
	}

	private static IEnumerable<SerializedObject> SerializedEntities(SerializedWorld serializedWorld)
	{
		return serializedWorld.Entities().Select(SerializeEntity);
	}

	private static SerializedObject SerializeEntity(SerializedEntity serializedEntity)
	{
		SerializedObject value = SerializedComponents(serializedEntity);
		SerializedObject serializedObject = new SerializedObject();
		serializedObject.Set("Id", serializedEntity.Id);
		serializedObject.Set("Template", serializedEntity.TemplateName);
		serializedObject.Set("Components", value);
		return serializedObject;
	}

	private static SerializedObject SerializedComponents(SerializedEntity serializedEntity)
	{
		SerializedObject serializedObject = new SerializedObject();
		foreach (string item in serializedEntity.Components())
		{
			SerializedComponent component = serializedEntity.GetComponent(item);
			serializedObject.Set(item, component.Value);
		}
		return serializedObject;
	}

	private static SerializedWorld DeserializeSave(SerializedObject save)
	{
		SerializedWorld serializedWorld = new SerializedWorld(Timberborn.Versioning.Version.Create(save.Get<string>("GameVersion")));
		SerializedObject serializedObject = save.Get<SerializedObject>("Singletons");
		SerializedObject[] array = save.GetArray<SerializedObject>("Entities");
		foreach (string item in serializedObject.Properties())
		{
			SerializedObject value = serializedObject.Get<SerializedObject>(item);
			serializedWorld.AddSingleton(new SerializedSingleton(item, value));
		}
		SerializedObject[] array2 = array;
		foreach (SerializedObject serializedObject2 in array2)
		{
			serializedWorld.AddEntity(DeserializeEntity(serializedObject2));
		}
		return serializedWorld;
	}

	private static SerializedEntity DeserializeEntity(SerializedObject serializedObject)
	{
		Guid id = serializedObject.Get<Guid>("Id");
		string templateName = (serializedObject.Has("Template") ? serializedObject.Get<string>("Template") : serializedObject.Get<string>("TemplateName"));
		SerializedEntity serializedEntity = new SerializedEntity(id, templateName);
		SerializedObject serializedObject2 = serializedObject.Get<SerializedObject>("Components");
		foreach (string item in serializedObject2.Properties())
		{
			serializedEntity.AddComponent(new SerializedComponent(item, serializedObject2.Get<SerializedObject>(item)));
		}
		return serializedEntity;
	}
}

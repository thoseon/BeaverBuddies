using System.IO;
using Timberborn.Common;
using Timberborn.SaveSystem;
using Timberborn.SerializationSystem;

namespace Timberborn.MapMetadataSystem;

public class MapMetadataSerializer : ISaveEntryReader<MapMetadata>
{
	private static readonly string WidthKey = "Width";

	private static readonly string HeightKey = "Height";

	private static readonly string MapNameLocKeyKey = "MapNameLocKey";

	private static readonly string MapDescriptionLocKeyKey = "MapDescriptionLocKey";

	private static readonly string MapDescriptionKey = "MapDescription";

	private static readonly string IsRecommendedKey = "IsRecommended";

	private static readonly string IsUnconventional = "IsUnconventional";

	private static readonly string IsDevKey = "IsDev";

	private readonly SerializedObjectReaderWriter _serializedObjectReaderWriter;

	public string EntryName => "map_metadata.json";

	public MapMetadataSerializer(SerializedObjectReaderWriter serializedObjectReaderWriter)
	{
		_serializedObjectReaderWriter = serializedObjectReaderWriter;
	}

	public void WriteToSaveEntryStream(Stream entryStream, MapMetadata mapMetadata)
	{
		_serializedObjectReaderWriter.WriteJson(GetMapMetadataSerializedObject(mapMetadata), entryStream);
	}

	public MapMetadata ReadFromSaveEntryStream(Stream entryStream)
	{
		return Deserialize(_serializedObjectReaderWriter.ReadJson(entryStream));
	}

	private static SerializedObject GetMapMetadataSerializedObject(MapMetadata mapMetadata)
	{
		SerializedObject serializedObject = new SerializedObject();
		serializedObject.Set(WidthKey, mapMetadata.Width);
		serializedObject.Set(HeightKey, mapMetadata.Height);
		serializedObject.Set(MapNameLocKeyKey, mapMetadata.MapNameLocKey);
		serializedObject.Set(MapDescriptionLocKeyKey, mapMetadata.MapDescriptionLocKey);
		serializedObject.Set(MapDescriptionKey, mapMetadata.MapDescription);
		serializedObject.Set(IsRecommendedKey, mapMetadata.IsRecommended);
		serializedObject.Set(IsUnconventional, mapMetadata.IsUnconventional);
		serializedObject.Set(IsDevKey, mapMetadata.IsDev);
		return serializedObject;
	}

	[BackwardCompatible(2025, 9, 25, Compatibility.Map)]
	private static MapMetadata Deserialize(SerializedObject serializedObject)
	{
		return new MapMetadata(serializedObject.Get<int>(WidthKey), serializedObject.Get<int>(HeightKey), serializedObject.Get<string>(MapNameLocKeyKey), serializedObject.Get<string>(MapDescriptionLocKeyKey), serializedObject.Get<string>(MapDescriptionKey), serializedObject.Get<bool>(IsRecommendedKey), serializedObject.GetOrDefault(IsUnconventional, defaultValue: false), serializedObject.Get<bool>(IsDevKey));
	}
}

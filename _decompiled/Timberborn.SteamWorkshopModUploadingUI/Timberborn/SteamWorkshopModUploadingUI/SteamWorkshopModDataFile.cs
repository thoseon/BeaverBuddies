using System.IO;
using Timberborn.Persistence;
using Timberborn.SerializationSystem;
using Timberborn.SteamWorkshop;

namespace Timberborn.SteamWorkshopModUploadingUI;

internal class SteamWorkshopModDataFile
{
	private static readonly string WorkshopDataFileName = "workshop_data.json";

	private readonly SteamWorkshopItemSerializer _steamWorkshopItemSerializer;

	private readonly SerializedObjectReaderWriter _serializedObjectReaderWriter;

	private readonly FileInfo _fileInfo;

	public SteamWorkshopItem SteamWorkshopItem { get; private set; }

	private SteamWorkshopModDataFile(SteamWorkshopItemSerializer steamWorkshopItemSerializer, SerializedObjectReaderWriter serializedObjectReaderWriter, FileInfo fileInfo)
	{
		_steamWorkshopItemSerializer = steamWorkshopItemSerializer;
		_serializedObjectReaderWriter = serializedObjectReaderWriter;
		_fileInfo = fileInfo;
	}

	public static SteamWorkshopModDataFile Create(SteamWorkshopItemSerializer steamWorkshopItemSerializer, SerializedObjectReaderWriter serializedObjectReaderWriter, string originPath)
	{
		FileInfo fileInfo = new FileInfo(Path.Combine(originPath, WorkshopDataFileName));
		SteamWorkshopModDataFile steamWorkshopModDataFile = new SteamWorkshopModDataFile(steamWorkshopItemSerializer, serializedObjectReaderWriter, fileInfo);
		steamWorkshopModDataFile.LoadFromFile();
		return steamWorkshopModDataFile;
	}

	public void SaveSteamWorkshopItem(SteamWorkshopItem steamWorkshopItem)
	{
		SteamWorkshopItem = steamWorkshopItem;
		File.WriteAllText(_fileInfo.FullName, GetSerializedData());
	}

	private void LoadFromFile()
	{
		if (_fileInfo.Exists)
		{
			string text = File.ReadAllText(_fileInfo.FullName);
			ValueLoader valueLoader = new ValueLoader(_serializedObjectReaderWriter.ReadJson(text));
			SteamWorkshopItem = _steamWorkshopItemSerializer.Deserialize(valueLoader).Value;
		}
	}

	private string GetSerializedData()
	{
		ValueSaver valueSaver = new ValueSaver();
		_steamWorkshopItemSerializer.Serialize(SteamWorkshopItem, valueSaver);
		return _serializedObjectReaderWriter.WriteJson((SerializedObject)valueSaver.Value);
	}
}

using System.IO;
using Timberborn.ErrorReporting;
using Timberborn.SaveSystem;
using Timberborn.WorldSerialization;

namespace Timberborn.GameSaveRepositorySystem;

public class GameSaveDeserializer
{
	private readonly GameSaveRepository _gameSaveRepository;

	private readonly SaveReader _saveReader;

	private readonly WorldSerializer _worldSerializer;

	public GameSaveDeserializer(GameSaveRepository gameSaveRepository, SaveReader saveReader, WorldSerializer worldSerializer)
	{
		_gameSaveRepository = gameSaveRepository;
		_saveReader = saveReader;
		_worldSerializer = worldSerializer;
	}

	public SerializedWorld Load(SaveReference saveReference)
	{
		string fileName = _gameSaveRepository.SaveNameToFileName(saveReference);
		using Stream stream = _gameSaveRepository.OpenSave(saveReference);
		WorldDataService.SetFromStream(fileName, stream);
		return _saveReader.ReadFromSaveStreamUnsafe(stream, _worldSerializer);
	}

	public T ReadFromSaveFile<T>(SaveReference saveReference, ISaveEntryReader<T> saveEntryReader)
	{
		using Stream saveStream = _gameSaveRepository.OpenSaveWithoutLogging(saveReference);
		return _saveReader.ReadFromSaveStream(saveStream, saveEntryReader);
	}

	public T ReadFromSaveFileUnsafe<T>(SaveReference saveReference, ISaveEntryReader<T> saveEntryReader)
	{
		using Stream saveStream = _gameSaveRepository.OpenSaveWithoutLogging(saveReference);
		return _saveReader.ReadFromSaveStreamUnsafe(saveStream, saveEntryReader);
	}
}

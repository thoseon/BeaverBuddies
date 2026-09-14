using System.IO;
using Timberborn.ErrorReporting;
using Timberborn.SaveSystem;
using Timberborn.WorldSerialization;

namespace Timberborn.MapRepositorySystem;

public class MapDeserializer
{
	private readonly MapRepository _mapRepository;

	private readonly SaveReader _saveReader;

	private readonly WorldSerializer _worldSerializer;

	public MapDeserializer(MapRepository mapRepository, SaveReader saveReader, WorldSerializer worldSerializer)
	{
		_mapRepository = mapRepository;
		_saveReader = saveReader;
		_worldSerializer = worldSerializer;
	}

	public SerializedWorld Load(MapFileReference mapFileReference)
	{
		using Stream stream = _mapRepository.OpenMap(mapFileReference);
		if (!mapFileReference.Resource)
		{
			WorldDataService.SetFromStream(_mapRepository.CustomMapNameToFileName(mapFileReference), stream);
		}
		return _saveReader.ReadFromSaveStreamUnsafe(stream, _worldSerializer);
	}

	public T ReadFromMapFile<T>(MapFileReference mapFileReference, ISaveEntryReader<T> saveEntryReader)
	{
		using Stream saveStream = _mapRepository.OpenMap(mapFileReference);
		return _saveReader.ReadFromSaveStream(saveStream, saveEntryReader);
	}

	public T ReadFromMapFileUnsafe<T>(MapFileReference mapFileReference, ISaveEntryReader<T> saveEntryReader)
	{
		using Stream saveStream = _mapRepository.OpenMap(mapFileReference);
		return _saveReader.ReadFromSaveStreamUnsafe(saveStream, saveEntryReader);
	}
}

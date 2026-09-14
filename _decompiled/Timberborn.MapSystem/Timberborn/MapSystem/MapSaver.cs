using System;
using System.IO;
using Timberborn.MapRepositorySystem;
using Timberborn.SaveSystem;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.MapSystem;

public class MapSaver
{
	private readonly MapRepository _mapRepository;

	private readonly SaveWriter _saveWriter;

	private readonly Ticker _ticker;

	public MapFileReference? LastSavedMap { get; private set; }

	public MapSaver(MapRepository mapRepository, SaveWriter saveWriter, Ticker ticker)
	{
		_mapRepository = mapRepository;
		_saveWriter = saveWriter;
		_ticker = ticker;
	}

	public void Save(MapFileReference mapFileReference)
	{
		string name = mapFileReference.Name;
		Debug.Log($"Saving map to {name} at {DateTime.Now:u}");
		_ticker.FinishFullTick();
		try
		{
			using MemoryStream memoryStream = new MemoryStream();
			_saveWriter.WriteToSaveStream(memoryStream, leaveOpen: true);
			using Stream destination = _mapRepository.CreateUserMap(name);
			memoryStream.Position = 0L;
			memoryStream.CopyTo(destination);
			LastSavedMap = mapFileReference;
		}
		catch (Exception ex) when (((ex is IOException || ex is ArgumentException) ? 1 : 0) != 0)
		{
			throw new MapSaverException(ex.Message, ex.InnerException);
		}
	}

	public void Save(Stream stream)
	{
		_saveWriter.WriteToSaveStream(stream);
	}

	public bool MapExists(string mapName)
	{
		try
		{
			return _mapRepository.UserMapExists(mapName);
		}
		catch (IOException ex)
		{
			throw new MapSaverException(ex.Message, ex.InnerException);
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Timberborn.AssetSystem;
using Timberborn.FileSystem;
using Timberborn.PlatformUtilities;
using Timberborn.SingletonSystem;

namespace Timberborn.MapRepositorySystem;

public class MapRepository
{
	private static readonly string MapsDirectory = "Maps";

	private static readonly string MapExtension = ".timber";

	private readonly IFileService _fileService;

	private readonly IAssetLoader _assetLoader;

	private readonly FilenameValidator _filenameValidator;

	private readonly EventBus _eventBus;

	public static string UserMapsDirectory => Path.Combine(UserDataFolder.Folder, MapsDirectory);

	public MapRepository(IFileService fileService, IAssetLoader assetLoader, FilenameValidator filenameValidator, EventBus eventBus)
	{
		_fileService = fileService;
		_assetLoader = assetLoader;
		_filenameValidator = filenameValidator;
		_eventBus = eventBus;
	}

	public Stream CreateUserMap(string mapName)
	{
		if (_filenameValidator.NameIsInvalid(mapName))
		{
			throw new ArgumentException(mapName + " contains an illegal character");
		}
		CreateUserMapsDirectory();
		string fileName = UserMapNameToFileName(mapName);
		return _fileService.CreateFile(fileName);
	}

	public bool UserMapExists(string mapName)
	{
		string fileName = UserMapNameToFileName(mapName);
		return _fileService.FileExists(fileName);
	}

	public Stream OpenMap(MapFileReference mapFileReference)
	{
		if (!mapFileReference.Resource)
		{
			return OpenDiskMap(mapFileReference);
		}
		return OpenResourceMap(mapFileReference);
	}

	public IEnumerable<string> GetBuiltinMapNames()
	{
		return from loadedAsset in _assetLoader.LoadAll<BinaryData>(MapsDirectory)
			select loadedAsset.Asset.name;
	}

	public IEnumerable<string> GetUserMapNames()
	{
		CreateUserMapsDirectory();
		return (from path in Directory.GetFiles(UserMapsDirectory)
			where Path.GetExtension(path) == MapExtension
			select path).Select(Path.GetFileNameWithoutExtension);
	}

	public IEnumerable<FileInfo> GetMapFilesFromDirectory(DirectoryInfo directoryInfo)
	{
		return from fileInfo in directoryInfo.GetFiles()
			where fileInfo.Extension == MapExtension
			select fileInfo;
	}

	public void DeleteMap(MapFileReference mapFileReference)
	{
		if (mapFileReference.UserFolder)
		{
			string fileName = CustomMapNameToFileName(mapFileReference);
			_fileService.DeleteFile(fileName);
			NotifyMapRepositoryChanged();
			return;
		}
		throw new NotSupportedException("Only user maps can be deleted.");
	}

	public void NotifyMapRepositoryChanged()
	{
		_eventBus.Post(new MapRepositoryChangedEvent());
	}

	public string CustomMapNameToFileName(MapFileReference mapFileReference)
	{
		if (!mapFileReference.UserFolder)
		{
			return mapFileReference.Path;
		}
		return UserMapNameToFileName(mapFileReference.Name);
	}

	public string MapNameWithExtension(string mapName)
	{
		return mapName + MapExtension;
	}

	private Stream OpenDiskMap(MapFileReference mapFileReference)
	{
		string fileName = CustomMapNameToFileName(mapFileReference);
		return _fileService.OpenFile(fileName);
	}

	private string UserMapNameToFileName(string mapName)
	{
		return _fileService.CombineIntoPath(UserMapsDirectory, MapNameWithExtension(mapName));
	}

	private void CreateUserMapsDirectory()
	{
		_fileService.CreateDirectory(UserMapsDirectory);
	}

	private Stream OpenResourceMap(MapFileReference mapFileReference)
	{
		string text = MapsDirectory + "/" + mapFileReference.Name;
		BinaryData binaryData = _assetLoader.Load<BinaryData>(text);
		if (binaryData == null)
		{
			throw new ArgumentException("Resource map " + text + " not found.");
		}
		return new MemoryStream(binaryData.Bytes);
	}
}

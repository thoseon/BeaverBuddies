using System;
using System.IO;
using Timberborn.Common;
using Timberborn.FileSystem;
using Timberborn.MapRepositorySystem;
using Timberborn.PlatformUtilities;
using UnityEngine;

namespace Timberborn.SteamWorkshopMapUploadingUI;

internal class SteamWorkshopMapContent
{
	private static readonly string WorkshopDirectoryName = "Workshop_temp";

	private static readonly string ThumbnailExtension = ".png";

	private readonly IFileService _fileService;

	private readonly MapRepository _mapRepository;

	private readonly MapFileReference _mapFileReference;

	public string ContentDirectory { get; private set; }

	public string ThumbnailPath { get; private set; }

	public Texture2D Thumbnail { get; }

	private static string WorkshopDirectory => Path.Combine(UserDataFolder.Folder, WorkshopDirectoryName);

	public SteamWorkshopMapContent(IFileService fileService, MapRepository mapRepository, Texture2D thumbnail, MapFileReference mapFileReference)
	{
		_fileService = fileService;
		_mapRepository = mapRepository;
		Thumbnail = thumbnail;
		_mapFileReference = mapFileReference;
	}

	public void CreateTemporaryFiles(string workshopMapName)
	{
		Asserts.FieldIsNull(this, ContentDirectory, "ContentDirectory");
		_fileService.CreateDirectory(WorkshopDirectory);
		Guid guid = Guid.NewGuid();
		CreateContentDirectory(guid, workshopMapName);
		CreateThumbnailFile(guid);
	}

	public void DeleteTemporaryFiles()
	{
		Asserts.FieldIsNotNull(this, ContentDirectory, "ContentDirectory");
		_fileService.DeleteDirectory(ContentDirectory);
		_fileService.DeleteFile(ThumbnailPath);
		ContentDirectory = null;
		ThumbnailPath = null;
	}

	private void CreateContentDirectory(Guid guid, string workshopMapName)
	{
		ContentDirectory = Path.Combine(WorkshopDirectory, guid.ToString());
		_fileService.CreateDirectory(ContentDirectory);
		string sourceFileName = _mapRepository.CustomMapNameToFileName(_mapFileReference);
		string destinationFileName = Path.Combine(ContentDirectory, _mapRepository.MapNameWithExtension(workshopMapName));
		_fileService.CopyFile(sourceFileName, destinationFileName);
	}

	private void CreateThumbnailFile(Guid guid)
	{
		string workshopDirectory = WorkshopDirectory;
		Guid guid2 = guid;
		ThumbnailPath = Path.Combine(workshopDirectory, guid2.ToString() + ThumbnailExtension);
		using Stream stream = _fileService.CreateFile(ThumbnailPath);
		byte[] array = Thumbnail.EncodeToPNG();
		stream.Write(array);
	}
}

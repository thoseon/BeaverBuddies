using System.IO;
using Timberborn.ErrorReporting;
using Timberborn.SaveSystem;
using Timberborn.SaveThumbnail;
using Timberborn.ThumbnailCapturing;

namespace Timberborn.SaveThumbnailCapturing;

internal class SaveThumbnailSaveEntryWriter : ISaveEntryWriter
{
	private readonly SaveThumbnailConfiguration _saveThumbnailConfiguration;

	private readonly ThumbnailSaveEntryWriter _thumbnailSaveEntryWriter;

	public string EntryName => _saveThumbnailConfiguration.Name;

	public SaveThumbnailSaveEntryWriter(SaveThumbnailConfiguration saveThumbnailConfiguration, ThumbnailSaveEntryWriter thumbnailSaveEntryWriter)
	{
		_saveThumbnailConfiguration = saveThumbnailConfiguration;
		_thumbnailSaveEntryWriter = thumbnailSaveEntryWriter;
	}

	public void WriteToSaveEntryStream(Stream entryStream)
	{
		if (!ErrorReporter.ErrorReported)
		{
			_thumbnailSaveEntryWriter.WriteToSaveEntryStream(entryStream, _saveThumbnailConfiguration);
		}
	}
}

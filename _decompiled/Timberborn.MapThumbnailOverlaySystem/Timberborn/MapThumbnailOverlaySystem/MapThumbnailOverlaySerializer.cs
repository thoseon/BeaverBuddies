using System.IO;
using Timberborn.SaveSystem;
using UnityEngine;

namespace Timberborn.MapThumbnailOverlaySystem;

public class MapThumbnailOverlaySerializer : ISaveEntryReader<byte[]>
{
	public string EntryName => "map_overlay.png";

	public void WriteToSaveEntryStream(Stream entryStream, Texture2D texture)
	{
		using StreamWriter streamWriter = new StreamWriter(entryStream);
		streamWriter.BaseStream.Write(texture.EncodeToPNG());
	}

	public byte[] ReadFromSaveEntryStream(Stream entryStream)
	{
		using MemoryStream memoryStream = new MemoryStream();
		entryStream.CopyTo(memoryStream);
		return memoryStream.ToArray();
	}
}

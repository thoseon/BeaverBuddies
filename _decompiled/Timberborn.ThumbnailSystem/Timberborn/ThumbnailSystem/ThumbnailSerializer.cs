using System.IO;
using Timberborn.TextureOperations;
using UnityEngine;

namespace Timberborn.ThumbnailSystem;

public class ThumbnailSerializer
{
	private readonly TextureFactory _textureFactory;

	public ThumbnailSerializer(TextureFactory textureFactory)
	{
		_textureFactory = textureFactory;
	}

	public void WriteToSaveEntryStream(Stream entryStream, Texture2D texture, IThumbnailConfiguration thumbnailConfiguration)
	{
		using StreamWriter streamWriter = new StreamWriter(entryStream);
		byte[] array = texture.EncodeToJPG(thumbnailConfiguration.Quality);
		streamWriter.BaseStream.Write(array);
	}

	public Texture2D ReadFromSaveEntryStream(Stream entryStream, IThumbnailConfiguration thumbnailConfiguration)
	{
		using MemoryStream memoryStream = new MemoryStream();
		entryStream.CopyTo(memoryStream);
		TextureSettings textureSettings = new TextureSettings.Builder().SetSize(thumbnailConfiguration.Width, thumbnailConfiguration.Height).SetTextureFormat(thumbnailConfiguration.TextureFormat).SetGenerateMipmap(generateMipmap: false)
			.Build();
		return _textureFactory.CreateTexture(textureSettings, memoryStream.ToArray());
	}
}

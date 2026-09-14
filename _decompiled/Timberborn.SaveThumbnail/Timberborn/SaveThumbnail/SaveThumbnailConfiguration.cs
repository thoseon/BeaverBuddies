using Timberborn.ThumbnailSystem;
using UnityEngine;

namespace Timberborn.SaveThumbnail;

public class SaveThumbnailConfiguration : IThumbnailConfiguration
{
	public int Width => 650;

	public int Height => 370;

	public int Quality => 80;

	public TextureFormat TextureFormat => TextureFormat.RGB24;

	public string Name => "save_thumbnail.jpg";
}

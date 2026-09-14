using Timberborn.ThumbnailSystem;
using UnityEngine;

namespace Timberborn.MapThumbnail;

public class MapThumbnailConfiguration : IThumbnailConfiguration
{
	public int Width => 960;

	public int Height => 540;

	public int Quality => 95;

	public TextureFormat TextureFormat => TextureFormat.RGB24;

	public string Name => "map_thumbnail.jpg";
}

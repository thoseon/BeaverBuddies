using UnityEngine;

namespace Timberborn.ThumbnailSystem;

public interface IThumbnailConfiguration
{
	int Width { get; }

	int Height { get; }

	int Quality { get; }

	TextureFormat TextureFormat { get; }
}

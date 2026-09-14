using UnityEngine;

namespace Timberborn.ThumbnailCapturing;

public interface IThumbnailRenderTextureProvider
{
	RenderTexture RenderTexture { get; }
}

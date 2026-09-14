using UnityEngine;

namespace Timberborn.TextureOperations;

public class TextureFactory
{
	public Texture2D CreateTexture(TextureSettings textureSettings)
	{
		Texture2D texture2D = (textureSettings.GenerateMipmap ? new Texture2D(textureSettings.Width, textureSettings.Height, textureSettings.TextureFormat, textureSettings.MipmapCount, textureSettings.Linear, createUninitialized: true) : new Texture2D(textureSettings.Width, textureSettings.Height, textureSettings.TextureFormat, mipChain: false, textureSettings.Linear, createUninitialized: true));
		ApplyTextureSettings(textureSettings, texture2D);
		return texture2D;
	}

	public Texture2D CreateTexture(TextureSettings textureSettings, byte[] bytes)
	{
		Texture2D texture2D = CreateTexture(textureSettings);
		texture2D.LoadImage(bytes);
		ApplyTextureSettings(textureSettings, texture2D);
		return texture2D;
	}

	public bool TryCreateTexture(TextureSettings textureSettings, byte[] bytes, out Texture2D texture)
	{
		texture = CreateTexture(textureSettings);
		if (texture.LoadImage(bytes))
		{
			ApplyTextureSettings(textureSettings, texture);
			return true;
		}
		Object.Destroy(texture);
		texture = null;
		return false;
	}

	public Texture2D CreateTexture(TextureSettings textureSettings, RenderTexture renderTexture)
	{
		Texture2D texture2D = CreateTexture(textureSettings);
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, texture2D.width, texture2D.height), 0, 0);
		RenderTexture.active = active;
		return texture2D;
	}

	private static void ApplyTextureSettings(TextureSettings textureSettings, Texture2D texture)
	{
		texture.anisoLevel = textureSettings.AnisoLevel;
		texture.filterMode = textureSettings.FilterMode;
		texture.wrapMode = textureSettings.WrapMode;
		texture.ignoreMipmapLimit = textureSettings.IgnoreMipmapLimits;
		texture.name = textureSettings.Name;
	}
}

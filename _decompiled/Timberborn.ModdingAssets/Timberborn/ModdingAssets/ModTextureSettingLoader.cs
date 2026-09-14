using System;
using System.IO;
using Timberborn.SerializationSystem;
using Timberborn.TextureOperations;
using UnityEngine;

namespace Timberborn.ModdingAssets;

internal class ModTextureSettingLoader
{
	public TextureSettings Load(FileInfo fileInfo, SerializedObject metadata)
	{
		TextureSettings.Builder builder = new TextureSettings.Builder().SetName(Path.GetFileNameWithoutExtension(fileInfo.Name));
		if (metadata.Has("isSprite") && metadata.Get<bool>("isSprite"))
		{
			builder.SetSpritePreset();
		}
		if (metadata.Has("isNormalMap") && metadata.Get<bool>("isNormalMap"))
		{
			builder.SetNormalMapPreset();
		}
		if (metadata.Has("linear"))
		{
			builder.SetLinear(metadata.Get<bool>("linear"));
		}
		if (metadata.Has("generateMipmap"))
		{
			builder.SetGenerateMipmap(metadata.Get<bool>("generateMipmap"));
		}
		if (metadata.Has("mipmapCount"))
		{
			builder.SetMipmapCount(metadata.Get<int>("mipmapCount"));
		}
		if (metadata.Has("ignoreMipmapLimits"))
		{
			builder.SetIgnoreMipmapLimits(metadata.Get<bool>("ignoreMipmapLimits"));
		}
		if (metadata.Has("filterMode") && Enum.TryParse<FilterMode>(metadata.Get<string>("filterMode"), out var result))
		{
			builder.SetFilterMode(result);
		}
		if (metadata.Has("wrapMode") && Enum.TryParse<TextureWrapMode>(metadata.Get<string>("wrapMode"), out var result2))
		{
			builder.SetWrapMode(result2);
		}
		if (metadata.Has("textureFormat") && Enum.TryParse<TextureFormat>(metadata.Get<string>("textureFormat"), out var result3))
		{
			builder.SetTextureFormat(result3);
		}
		if (metadata.Has("anisoLevel"))
		{
			builder.SetAnisoLevel(metadata.Get<int>("anisoLevel"));
		}
		if (metadata.Has("width") && metadata.Has("height"))
		{
			builder.SetSize(metadata.Get<int>("width"), metadata.Get<int>("height"));
		}
		return builder.Build();
	}
}

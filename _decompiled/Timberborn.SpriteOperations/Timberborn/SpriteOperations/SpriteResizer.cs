using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using Timberborn.TextureOperations;
using Unity.Collections;
using UnityEngine;

namespace Timberborn.SpriteOperations;

public class SpriteResizer : IUnloadableSingleton
{
	private readonly struct TargetSprite : IEquatable<TargetSprite>
	{
		public Sprite Original { get; }

		public int Size { get; }

		public bool HasMipmaps => Original.texture.mipmapCount > 1;

		public TargetSprite(Sprite original, int size)
		{
			Original = original;
			Size = size;
		}

		public bool Equals(TargetSprite other)
		{
			if (object.Equals(Original, other.Original))
			{
				return Size == other.Size;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is TargetSprite other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Original, Size);
		}
	}

	private readonly TextureFactory _textureFactory;

	private readonly Dictionary<TargetSprite, Sprite> _spritesMap = new Dictionary<TargetSprite, Sprite>();

	private readonly List<Sprite> _createdSprites = new List<Sprite>();

	public SpriteResizer(TextureFactory textureFactory)
	{
		_textureFactory = textureFactory;
	}

	public void Unload()
	{
		for (int i = 0; i < _createdSprites.Count; i++)
		{
			UnityEngine.Object.Destroy(_createdSprites[i].texture);
			UnityEngine.Object.Destroy(_createdSprites[i]);
		}
		_spritesMap.Clear();
		_createdSprites.Clear();
	}

	public Sprite GetResizedSprite(Sprite original, int size)
	{
		TargetSprite targetSprite = new TargetSprite(original, size);
		return _spritesMap.GetOrAdd(targetSprite, () => GetResizedSprite(targetSprite));
	}

	private Sprite GetResizedSprite(TargetSprite targetSprite)
	{
		if (targetSprite.HasMipmaps)
		{
			return CreateResizedSprite(targetSprite);
		}
		return targetSprite.Original;
	}

	private Sprite CreateResizedSprite(TargetSprite targetSprite)
	{
		Texture2D texture = CreateResizedTexture(targetSprite);
		Sprite original = targetSprite.Original;
		Sprite sprite = Sprite.Create(texture, original.rect, original.pivot, original.pixelsPerUnit);
		_createdSprites.Add(sprite);
		return sprite;
	}

	private Texture2D CreateResizedTexture(TargetSprite targetSprite)
	{
		Texture2D texture = targetSprite.Original.texture;
		int mipMapCount = GetMipMapCount(targetSprite, texture);
		TextureSettings textureSettings = new TextureSettings.Builder().SetSize(texture.width, texture.height).SetTextureFormat(texture.format).SetMipmapCount(mipMapCount)
			.SetIgnoreMipmapLimits(ignoreMipmapLimits: true)
			.Build();
		Texture2D texture2D = _textureFactory.CreateTexture(textureSettings);
		for (int i = 0; i < texture2D.mipmapCount; i++)
		{
			NativeArray<Color32> pixelData = texture.GetPixelData<Color32>(i);
			texture2D.SetPixelData(pixelData, i);
		}
		texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: true);
		return texture2D;
	}

	private static int GetMipMapCount(TargetSprite targetSprite, Texture2D originalTexture)
	{
		return Math.Clamp((int)Math.Ceiling(Math.Log(originalTexture.width / (targetSprite.Size - 1)) / Math.Log(2.0)) - 1, 0, originalTexture.mipmapCount - 1) + 1;
	}
}

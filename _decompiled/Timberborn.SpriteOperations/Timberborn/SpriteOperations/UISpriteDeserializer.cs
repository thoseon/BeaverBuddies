using System;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.SpriteOperations;

internal class UISpriteDeserializer : IDeserializer
{
	private static readonly int UISpriteSize = 24;

	private readonly SpriteResizer _spriteResizer;

	public Type DeserializedType => typeof(UISprite);

	public UISpriteDeserializer(SpriteResizer spriteResizer)
	{
		_spriteResizer = spriteResizer;
	}

	public object Deserialize(object source)
	{
		Sprite asset = ((AssetRef<Sprite>)source).Asset;
		return new UISprite(_spriteResizer.GetResizedSprite(asset, UISpriteSize));
	}
}

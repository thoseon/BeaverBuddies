using System;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.SpriteOperations;

internal class FlippedSpriteDeserializer : IDeserializer
{
	private readonly SpriteFlipper _spriteFlipper;

	public Type DeserializedType => typeof(FlippedSprite);

	public FlippedSpriteDeserializer(SpriteFlipper spriteFlipper)
	{
		_spriteFlipper = spriteFlipper;
	}

	public object Deserialize(object source)
	{
		Sprite asset = ((AssetRef<Sprite>)source).Asset;
		return new FlippedSprite(_spriteFlipper.GetFlippedSprite(asset));
	}
}

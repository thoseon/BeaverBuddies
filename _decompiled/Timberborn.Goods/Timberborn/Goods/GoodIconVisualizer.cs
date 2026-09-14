using UnityEngine;

namespace Timberborn.Goods;

public class GoodIconVisualizer
{
	private static readonly int ColorProperty = Shader.PropertyToID("_Color");

	private static readonly int IconColorProperty = Shader.PropertyToID("_DetailAlbedoUV2Color");

	private static readonly int TextureProperty = Shader.PropertyToID("_DetailAlbedoMap2");

	public void ShowIcon(Material material, GoodSpec goodSpec)
	{
		ShowIcon(material, goodSpec, goodSpec.ContainerColor);
	}

	public void ShowIcon(Material material, GoodSpec goodSpec, Color color)
	{
		UpdateIconParameters(material, goodSpec, flipped: false, ColorProperty, color);
	}

	public void ShowColoredIcon(Material material, GoodSpec goodSpec, bool flipped, Color color)
	{
		UpdateIconParameters(material, goodSpec, flipped, IconColorProperty, color);
	}

	public void HideColoredIcon(Material material)
	{
		ClearIconParameters(material, IconColorProperty);
	}

	private static void UpdateIconParameters(Material material, GoodSpec goodSpec, bool flipped, int colorProperty, Color color)
	{
		if (color.a > 0f)
		{
			Sprite sprite = (flipped ? goodSpec.IconFlipped.Value : goodSpec.Icon.Asset);
			SetIconParameters(material, sprite.texture, colorProperty, color);
		}
		else
		{
			ClearIconParameters(material, colorProperty);
		}
	}

	private static void SetIconParameters(Material material, Texture texture, int colorProperty, Color color)
	{
		material.SetTexture(TextureProperty, texture);
		material.SetColor(colorProperty, color);
	}

	private static void ClearIconParameters(Material material, int colorProperty)
	{
		SetIconParameters(material, Texture2D.blackTexture, colorProperty, Color.white);
	}
}

using Timberborn.AssetSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.Common;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.CharacterModelSystem;

internal class CharacterTextureSetter : BaseComponent, IAwakableComponent, IPostInitializableEntity
{
	private static readonly int DiffuseMapId = Shader.PropertyToID("_BaseMap");

	private static readonly int EmissionMapId = Shader.PropertyToID("_EmissionMap");

	private static readonly int NormalMapId = Shader.PropertyToID("_BumpMap");

	private static readonly int DisplacementMapId = Shader.PropertyToID("_BeeStingMap");

	private static readonly int DisplacementScaleId = Shader.PropertyToID("_BeeSting");

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly IAssetLoader _assetLoader;

	private CharacterMaterialModifier _characterMaterialModifier;

	public CharacterTextureSetter(IRandomNumberGenerator randomNumberGenerator, IAssetLoader assetLoader)
	{
		_randomNumberGenerator = randomNumberGenerator;
		_assetLoader = assetLoader;
	}

	public void Awake()
	{
		_characterMaterialModifier = GetComponent<CharacterMaterialModifier>();
	}

	public void PostInitializeEntity()
	{
		CharacterTextureSetterSpec component = GetComponent<CharacterTextureSetterSpec>();
		CharacterTexturePack enumerableElement = _randomNumberGenerator.GetEnumerableElement(component.TexturePacks);
		SetTexture(enumerableElement.DiffuseTexture, DiffuseMapId);
		SetTexture(enumerableElement.EmissionTexture, EmissionMapId);
		SetTexture(enumerableElement.NormalTexture, NormalMapId);
		SetTexture(enumerableElement.DisplacementTexture, DisplacementMapId);
		_characterMaterialModifier.SetFloat(DisplacementScaleId, 3f);
	}

	private void SetTexture(string textureName, int textureId)
	{
		if (!string.IsNullOrEmpty(textureName))
		{
			Texture2D texture = _assetLoader.Load<Texture2D>(textureName);
			_characterMaterialModifier.SetTexture(textureId, texture);
		}
	}
}

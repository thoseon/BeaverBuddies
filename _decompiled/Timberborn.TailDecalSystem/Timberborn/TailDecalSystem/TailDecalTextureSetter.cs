using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using UnityEngine;

namespace Timberborn.TailDecalSystem;

internal class TailDecalTextureSetter : BaseComponent, IAwakableComponent
{
	private static readonly int DecalDiffusePropertyId = Shader.PropertyToID("_TailDecalDiffuse");

	private CharacterMaterialModifier _characterMaterialModifier;

	public void Awake()
	{
		_characterMaterialModifier = GetComponent<CharacterMaterialModifier>();
	}

	public void SetTexture(Texture texture)
	{
		_characterMaterialModifier.SetTexture(DecalDiffusePropertyId, texture);
	}

	public void ClearDecalTexture()
	{
		_characterMaterialModifier.SetTexture(DecalDiffusePropertyId, null);
	}
}

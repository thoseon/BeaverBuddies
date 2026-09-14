using Timberborn.BaseComponentSystem;
using UnityEngine;

namespace Timberborn.Characters;

public class CharacterTint : BaseComponent, IAwakableComponent, IChildhoodInfluenced
{
	private static readonly int TintColorId = Shader.PropertyToID("_TintColor");

	private static readonly int TintEnabledId = Shader.PropertyToID("_TintEnabled");

	private CharacterMaterialModifier _characterMaterialModifier;

	private bool _isEnabled;

	private Color _tintColor;

	public void Awake()
	{
		_characterMaterialModifier = GetComponent<CharacterMaterialModifier>();
	}

	public void SetTint(Color tintColor)
	{
		_isEnabled = true;
		_tintColor = tintColor;
		UpdateMaterialProperties();
	}

	public void DisableTint()
	{
		_isEnabled = false;
		UpdateMaterialProperties();
	}

	public void InfluenceByChildhood(Character child)
	{
		CharacterTint component = child.GetComponent<CharacterTint>();
		if ((bool)component)
		{
			CopyFrom(component);
		}
	}

	private void UpdateMaterialProperties()
	{
		_characterMaterialModifier.SetColor(TintColorId, _tintColor);
		_characterMaterialModifier.SetFloat(TintEnabledId, _isEnabled ? 1f : 0f);
	}

	private void CopyFrom(CharacterTint characterTint)
	{
		_isEnabled = characterTint._isEnabled;
		_tintColor = characterTint._tintColor;
		UpdateMaterialProperties();
	}
}

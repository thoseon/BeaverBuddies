using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.EntitySystem;
using Timberborn.MortalComponents;
using Timberborn.NeedSystem;
using UnityEngine;

namespace Timberborn.Healthcare;

internal class BeaverNeedShaderPropertySetter : BaseComponent, IAwakableComponent, IDeadNeededComponent, IInitializableEntity
{
	private CharacterMaterialModifier _characterMaterialModifier;

	private BeaverNeedShaderPropertySetterSpec _beaverNeedShaderPropertySetterSpec;

	private Dictionary<BeaverNeedShaderPropertySet, int> _propertyIds;

	public void Awake()
	{
		_characterMaterialModifier = GetComponent<CharacterMaterialModifier>();
		_beaverNeedShaderPropertySetterSpec = GetComponent<BeaverNeedShaderPropertySetterSpec>();
		_propertyIds = _beaverNeedShaderPropertySetterSpec.PropertySets.ToDictionary((BeaverNeedShaderPropertySet s) => s, (BeaverNeedShaderPropertySet s) => Shader.PropertyToID(s.PropertyName));
		GetComponent<NeedManager>().NeedChangedActiveState += OnNeedChangedActiveState;
	}

	public void InitializeEntity()
	{
		UpdateAllParameters();
	}

	private void OnNeedChangedActiveState(object sender, NeedChangedActiveStateEventArgs e)
	{
		foreach (BeaverNeedShaderPropertySet propertySet in _beaverNeedShaderPropertySetterSpec.PropertySets)
		{
			if (e.NeedSpec.Id == propertySet.NeedId)
			{
				UpdateParameter(propertySet, e.IsActive);
			}
		}
	}

	private void UpdateAllParameters()
	{
		foreach (BeaverNeedShaderPropertySet propertySet in _beaverNeedShaderPropertySetterSpec.PropertySets)
		{
			bool isNeedActive = GetComponent<NeedManager>().NeedIsActive(propertySet.NeedId);
			UpdateParameter(propertySet, isNeedActive);
		}
	}

	private void UpdateParameter(BeaverNeedShaderPropertySet propertySet, bool isNeedActive)
	{
		_characterMaterialModifier.SetFloat(_propertyIds[propertySet], isNeedActive ? 1 : 0);
	}
}

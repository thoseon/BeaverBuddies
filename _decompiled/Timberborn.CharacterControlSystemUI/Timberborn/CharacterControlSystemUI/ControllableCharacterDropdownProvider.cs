using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CharacterControlSystem;
using Timberborn.DropdownSystem;
using Timberborn.TemplateSystem;

namespace Timberborn.CharacterControlSystemUI;

internal class ControllableCharacterDropdownProvider : BaseComponent, IAwakableComponent, IDropdownProvider
{
	private readonly ControllableCharacterAnimations _controllableCharacterAnimations;

	private ControllableCharacter _controllableCharacter;

	private TemplateSpec _templateSpec;

	public IReadOnlyList<string> Items => _controllableCharacterAnimations.GatAnimations(_controllableCharacter, _templateSpec.TemplateName);

	public ControllableCharacterDropdownProvider(ControllableCharacterAnimations controllableCharacterAnimations)
	{
		_controllableCharacterAnimations = controllableCharacterAnimations;
	}

	public void Awake()
	{
		_controllableCharacter = GetComponent<ControllableCharacter>();
		_templateSpec = GetComponent<TemplateSpec>();
	}

	public string GetValue()
	{
		return _controllableCharacter.WaitAnimation;
	}

	public void SetInitialAnimation()
	{
		SetValue(_controllableCharacter.UnderControl ? _controllableCharacter.WaitAnimation : Items[0]);
	}

	public void SetValue(string value)
	{
		_controllableCharacter.ChangeAnimation(value);
	}
}

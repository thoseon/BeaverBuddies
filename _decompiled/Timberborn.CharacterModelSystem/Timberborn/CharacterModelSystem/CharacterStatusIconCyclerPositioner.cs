using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.StatusSystem;
using UnityEngine;

namespace Timberborn.CharacterModelSystem;

internal class CharacterStatusIconCyclerPositioner : BaseComponent, IAwakableComponent, ILateUpdatableComponent, IInitializableEntity
{
	private CharacterModel _characterModel;

	private Transform _statusIconCyclerTransform;

	private Vector3 _iconOffset;

	public void Awake()
	{
		_characterModel = GetComponent<CharacterModel>();
		DisableComponent();
	}

	public void InitializeEntity()
	{
		StatusIconCycler component = GetComponent<StatusIconCycler>();
		_statusIconCyclerTransform = component.Root.transform;
		_iconOffset = _statusIconCyclerTransform.localPosition;
		EnableComponent();
	}

	public void LateUpdate()
	{
		_statusIconCyclerTransform.position = _characterModel.Position + _iconOffset;
	}
}

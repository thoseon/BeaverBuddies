using Timberborn.BaseComponentSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.GameDistrictsUI;

internal class DistrictCenterEntityBadge : BaseComponent, IAwakableComponent, IEntityBadge
{
	private LabeledEntity _labeledEntity;

	public int EntityBadgePriority => 120;

	public void Awake()
	{
		_labeledEntity = GetComponent<LabeledEntity>();
	}

	public string GetEntitySubtitle()
	{
		return _labeledEntity.DisplayName;
	}

	public ClickableSubtitle GetEntityClickableSubtitle()
	{
		return ClickableSubtitle.CreateEmpty();
	}

	public Sprite GetEntityAvatar()
	{
		return _labeledEntity.Image;
	}
}

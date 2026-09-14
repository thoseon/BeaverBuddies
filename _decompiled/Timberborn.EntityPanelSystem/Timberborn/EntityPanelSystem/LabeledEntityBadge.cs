using Timberborn.BaseComponentSystem;
using Timberborn.EntityNaming;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.EntityPanelSystem;

public class LabeledEntityBadge : BaseComponent, IAwakableComponent, IEntityBadge
{
	private LabeledEntity _labeledEntity;

	private NamedEntity _namedEntity;

	private Sprite _image;

	public int EntityBadgePriority => 0;

	public void Awake()
	{
		_labeledEntity = GetComponent<LabeledEntity>();
		_namedEntity = GetComponent<NamedEntity>();
	}

	public string GetEntitySubtitle()
	{
		NamedEntity namedEntity = _namedEntity;
		if (namedEntity == null || !namedEntity.IsEditable)
		{
			return "";
		}
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

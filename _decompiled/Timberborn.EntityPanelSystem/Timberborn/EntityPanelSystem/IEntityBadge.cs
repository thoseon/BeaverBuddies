using UnityEngine;

namespace Timberborn.EntityPanelSystem;

public interface IEntityBadge
{
	bool EntityBadgeEnabled => true;

	int EntityBadgePriority { get; }

	string GetEntitySubtitle();

	ClickableSubtitle GetEntityClickableSubtitle();

	Sprite GetEntityAvatar();
}

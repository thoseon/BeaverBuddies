using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.EntityPanelSystem;

public class EntityBadgeService : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly List<IEntityBadge> _entityBadgesCache = new List<IEntityBadge>();

	public EntityBadgeService(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public string GetEntitySubtitle(BaseComponent subject)
	{
		return GetHighestPriorityEntityBadge(subject)?.GetEntitySubtitle() ?? "";
	}

	public ClickableSubtitle GetEntityClickableSubtitle(BaseComponent subject)
	{
		return GetHighestPriorityEntityBadge(subject)?.GetEntityClickableSubtitle() ?? ClickableSubtitle.CreateEmpty();
	}

	public Sprite GetEntityAvatar(BaseComponent subject)
	{
		return GetHighestPriorityEntityBadge(subject)?.GetEntityAvatar();
	}

	private IEntityBadge GetHighestPriorityEntityBadge(BaseComponent subject)
	{
		subject.GetComponents(_entityBadgesCache);
		return GetHighestPriorityEntityBadge();
	}

	private IEntityBadge GetHighestPriorityEntityBadge()
	{
		if (_entityBadgesCache.Count > 0)
		{
			int num = int.MinValue;
			IEntityBadge result = null;
			foreach (IEntityBadge item in _entityBadgesCache)
			{
				if (item.EntityBadgeEnabled && item.EntityBadgePriority > num)
				{
					num = item.EntityBadgePriority;
					result = item;
				}
			}
			_entityBadgesCache.Clear();
			return result;
		}
		return null;
	}
}

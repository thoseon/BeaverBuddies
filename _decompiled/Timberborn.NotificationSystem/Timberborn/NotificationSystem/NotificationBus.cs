using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.GameCycleSystem;

namespace Timberborn.NotificationSystem;

public class NotificationBus
{
	private readonly GameCycleService _gameCycleService;

	public event EventHandler<NotificationEventArgs> NotificationPosted;

	public NotificationBus(GameCycleService gameCycleService)
	{
		_gameCycleService = gameCycleService;
	}

	public void Post(string description, BaseComponent subject)
	{
		EntityComponent component = subject.GetComponent<EntityComponent>();
		Post(description, component);
	}

	private void Post(string description, EntityComponent entityComponent)
	{
		Guid entityId = entityComponent.EntityId;
		Notification notification = new Notification(description, entityId, _gameCycleService.Cycle, _gameCycleService.CycleDay);
		NotificationPosted?.Invoke(this, new NotificationEventArgs(notification));
	}
}

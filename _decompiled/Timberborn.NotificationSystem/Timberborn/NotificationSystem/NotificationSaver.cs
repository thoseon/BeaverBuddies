using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.NotificationSystem;

public class NotificationSaver : ISaveableSingleton, ILoadableSingleton
{
	public static readonly int MaxNotifications = 25;

	private static readonly SingletonKey NotificationSaverKey = new SingletonKey("NotificationSaver");

	private static readonly ListKey<Notification> NotificationsKey = new ListKey<Notification>("Notifications");

	private readonly ISingletonLoader _singletonLoader;

	private readonly NotificationBus _notificationBus;

	private readonly NotificationValueSerializer _notificationValueSerializer;

	private readonly Queue<Notification> _notifications = new Queue<Notification>();

	public IEnumerable<Notification> Notifications => _notifications.AsReadOnlyEnumerable();

	public NotificationSaver(ISingletonLoader singletonLoader, NotificationBus notificationBus, NotificationValueSerializer notificationValueSerializer)
	{
		_singletonLoader = singletonLoader;
		_notificationBus = notificationBus;
		_notificationValueSerializer = notificationValueSerializer;
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(NotificationSaverKey, out var objectLoader))
		{
			foreach (Notification item in objectLoader.Get(NotificationsKey, _notificationValueSerializer))
			{
				AddNotification(item);
			}
		}
		_notificationBus.NotificationPosted += delegate(object _, NotificationEventArgs args)
		{
			AddNotification(args.Notification);
		};
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		IObjectSaver singleton = singletonSaver.GetSingleton(NotificationSaverKey);
		List<Notification> list = new List<Notification>();
		foreach (Notification notification in _notifications)
		{
			list.Add(notification);
		}
		singleton.Set(NotificationsKey, list, _notificationValueSerializer);
	}

	private void AddNotification(Notification notification)
	{
		if (_notifications.Count == MaxNotifications)
		{
			_notifications.Dequeue();
		}
		_notifications.Enqueue(notification);
	}
}

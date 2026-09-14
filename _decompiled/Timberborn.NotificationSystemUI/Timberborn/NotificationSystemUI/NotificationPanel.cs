using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.NotificationSystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.UIFormatters;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.NotificationSystemUI;

internal class NotificationPanel : IPostLoadableSingleton
{
	private static readonly string HiddenClass = "extension-clamp-full--hidden";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly UILayout _uiLayout;

	private readonly NotificationBus _notificationBus;

	private readonly NotificationSaver _notificationSaver;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly EntityRegistry _entityRegistry;

	private readonly TimestampFormatter _timestampFormatter;

	private readonly EventBus _eventBus;

	private readonly Queue<VisualElement> _notifications = new Queue<VisualElement>();

	private VisualElement _root;

	private ScrollView _notificationView;

	private Notification _latestNotification;

	private VisualElement _latestNotificationElement;

	private Button _extensionToggler;

	private bool _extended = true;

	public NotificationPanel(VisualElementLoader visualElementLoader, UILayout uiLayout, NotificationBus notificationBus, NotificationSaver notificationSaver, EntitySelectionService entitySelectionService, EntityRegistry entityRegistry, TimestampFormatter timestampFormatter, EventBus eventBus)
	{
		_visualElementLoader = visualElementLoader;
		_uiLayout = uiLayout;
		_notificationBus = notificationBus;
		_notificationSaver = notificationSaver;
		_entitySelectionService = entitySelectionService;
		_entityRegistry = entityRegistry;
		_timestampFormatter = timestampFormatter;
		_eventBus = eventBus;
	}

	public void PostLoad()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/NotificationPanel/NotificationPanel");
		_notificationView = _root.Q<ScrollView>("Notifications");
		_latestNotificationElement = _root.Q<VisualElement>("LatestNotification");
		_latestNotificationElement.RegisterCallback<ClickEvent>(delegate
		{
			FocusOnLatestNotification();
		});
		_latestNotificationElement.ToggleDisplayStyle(visible: false);
		_extensionToggler = _root.Q<Button>("ExtensionToggler");
		_extensionToggler.RegisterCallback<ClickEvent>(ToggleSelection);
		foreach (Notification notification in _notificationSaver.Notifications)
		{
			AddNotification(notification);
		}
		_notificationBus.NotificationPosted += delegate(object _, NotificationEventArgs args)
		{
			AddNotification(args.Notification);
		};
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_uiLayout.AddTopLeft(_root, 3);
	}

	private void ToggleSelection(ClickEvent evt)
	{
		if (_extended)
		{
			_notificationView.ToggleDisplayStyle(visible: false);
			_latestNotificationElement.ToggleDisplayStyle(visible: true);
			_extensionToggler.AddToClassList(HiddenClass);
			_extended = false;
		}
		else
		{
			_notificationView.ToggleDisplayStyle(visible: true);
			_latestNotificationElement.ToggleDisplayStyle(visible: false);
			_extensionToggler.RemoveFromClassList(HiddenClass);
			_extended = true;
		}
	}

	private void AddNotification(Notification notification)
	{
		if (_notifications.Count == NotificationSaver.MaxNotifications)
		{
			_notificationView.Remove(_notifications.Dequeue());
		}
		_latestNotification = notification;
		_latestNotificationElement.Clear();
		_latestNotificationElement.Add(CreateNotificationElement(notification));
		VisualElement visualElement = CreateNotificationElement(notification);
		_notifications.Enqueue(visualElement);
		_notificationView.Add(visualElement);
	}

	private VisualElement CreateNotificationElement(Notification notification)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/NotificationPanel/NotificationPanelItem");
		visualElement.Q<Label>("Date").text = _timestampFormatter.FormatShort(notification.Cycle, notification.CycleDay);
		visualElement.Q<Label>("Text").text = notification.Description;
		visualElement.RegisterCallback<ClickEvent>(delegate
		{
			FocusOnNotification(notification);
		});
		return visualElement;
	}

	private void FocusOnLatestNotification()
	{
		if (_latestNotification != null)
		{
			FocusOnNotification(_latestNotification);
		}
	}

	private void FocusOnNotification(Notification notification)
	{
		EntityComponent entity = _entityRegistry.GetEntity(notification.Subject);
		if (entity != null)
		{
			_entitySelectionService.SelectAndFollow(entity);
		}
	}
}

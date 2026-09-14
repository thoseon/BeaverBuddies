using Timberborn.InputSystem;
using Timberborn.QuickNotificationSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.InputSystemUI;

internal class KeywordMatchNotifier : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly QuickNotificationService _quickNotificationService;

	public KeywordMatchNotifier(EventBus eventBus, QuickNotificationService quickNotificationService)
	{
		_eventBus = eventBus;
		_quickNotificationService = quickNotificationService;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnKeywordMatched(KeywordMatchedEvent keywordMatchedEvent)
	{
		_quickNotificationService.SendNotification(keywordMatchedEvent.KeywordNotification);
	}
}

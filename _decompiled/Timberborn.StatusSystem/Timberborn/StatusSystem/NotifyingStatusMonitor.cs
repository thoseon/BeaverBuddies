using System;
using System.Collections.Generic;
using Timberborn.SingletonSystem;

namespace Timberborn.StatusSystem;

internal class NotifyingStatusMonitor
{
	private readonly EventBus _eventBus;

	private readonly HashSet<StatusSubject> _statusSubjects = new HashSet<StatusSubject>();

	public NotifyingStatusMonitor(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void AddSubject(StatusSubject statusSubject)
	{
		if (_statusSubjects.Add(statusSubject))
		{
			statusSubject.StatusToggled += OnStatusToggled;
			return;
		}
		throw new ArgumentException("StatusSubject " + statusSubject.Name + " is already added.");
	}

	public void RemoveSubject(StatusSubject statusSubject)
	{
		statusSubject.StatusToggled -= OnStatusToggled;
		_statusSubjects.Remove(statusSubject);
	}

	private void OnStatusToggled(object sender, StatusInstance statusInstance)
	{
		if (statusInstance.IsNotifying)
		{
			_eventBus.Post(new NotifyingStatusChangedEvent(statusInstance.AlertDescription));
		}
	}
}

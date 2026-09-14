using System;

namespace Timberborn.NotificationSystem;

public class Notification
{
	public string Description { get; }

	public Guid Subject { get; }

	public int Cycle { get; }

	public int CycleDay { get; }

	public Notification(string description, Guid subject, int cycle, int cycleDay)
	{
		Description = description;
		Subject = subject;
		Cycle = cycle;
		CycleDay = cycleDay;
	}
}

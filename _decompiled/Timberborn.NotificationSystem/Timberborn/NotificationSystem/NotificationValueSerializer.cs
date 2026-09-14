using System;
using Timberborn.Persistence;

namespace Timberborn.NotificationSystem;

public class NotificationValueSerializer : IValueSerializer<Notification>
{
	private static readonly PropertyKey<string> DescriptionKey = new PropertyKey<string>("Description");

	private static readonly PropertyKey<Guid> SubjectKey = new PropertyKey<Guid>("Subject");

	private static readonly PropertyKey<int> CycleKey = new PropertyKey<int>("Cycle");

	private static readonly PropertyKey<int> CycleDayKey = new PropertyKey<int>("CycleDay");

	public void Serialize(Notification value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(DescriptionKey, value.Description);
		objectSaver.Set(SubjectKey, value.Subject);
		objectSaver.Set(CycleKey, value.Cycle);
		objectSaver.Set(CycleDayKey, value.CycleDay);
	}

	public Obsoletable<Notification> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		string description = objectLoader.Get(DescriptionKey);
		Guid subject = objectLoader.Get(SubjectKey);
		int cycle = objectLoader.Get(CycleKey);
		int cycleDay = objectLoader.Get(CycleDayKey);
		return new Notification(description, subject, cycle, cycleDay);
	}
}

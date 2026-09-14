using Timberborn.Persistence;

namespace Timberborn.AutomationBuildings;

public class TimerIntervalSerializer : IValueSerializer<TimerInterval>
{
	private static readonly PropertyKey<IntervalType> TypeKey = new PropertyKey<IntervalType>("Type");

	private static readonly PropertyKey<int> TicksKey = new PropertyKey<int>("Ticks");

	private static readonly PropertyKey<float> HoursKey = new PropertyKey<float>("Hours");

	private readonly TimerIntervalFactory _timerIntervalFactory;

	public TimerIntervalSerializer(TimerIntervalFactory timerIntervalFactory)
	{
		_timerIntervalFactory = timerIntervalFactory;
	}

	public void Serialize(TimerInterval value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		if (value.TryGetHours(out var hours))
		{
			objectSaver.Set(TypeKey, value.Type);
			objectSaver.Set(HoursKey, hours);
		}
		else
		{
			objectSaver.Set(TicksKey, value.Ticks);
		}
	}

	public Obsoletable<TimerInterval> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		if (objectLoader.Has(HoursKey))
		{
			return _timerIntervalFactory.CreateFromHours(objectLoader.Get(HoursKey), objectLoader.Get(TypeKey));
		}
		return _timerIntervalFactory.CreateFromTicks(objectLoader.Get(TicksKey));
	}
}

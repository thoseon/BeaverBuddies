using Timberborn.Persistence;

namespace Timberborn.Workshops;

public class HourlyProductivitySerializer : IValueSerializer<HourlyProductivity>
{
	private static readonly PropertyKey<int> MaxWorkPotentialKey = new PropertyKey<int>("MaxWorkPotential");

	private static readonly PropertyKey<int> ActualWorkPerformedKey = new PropertyKey<int>("ActualWorkPerformed");

	private static readonly PropertyKey<bool> WasWorkingHourKey = new PropertyKey<bool>("WasWorkingHour");

	public void Serialize(HourlyProductivity value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(MaxWorkPotentialKey, value.MaxWorkPotential);
		objectSaver.Set(ActualWorkPerformedKey, value.ActualWorkPerformed);
		objectSaver.Set(WasWorkingHourKey, value.WasWorkingHour);
	}

	public Obsoletable<HourlyProductivity> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		return new HourlyProductivity(objectLoader.Get(MaxWorkPotentialKey), objectLoader.Get(ActualWorkPerformedKey), objectLoader.Get(WasWorkingHourKey));
	}
}

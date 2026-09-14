using Timberborn.Common;

namespace Timberborn.Workshops;

public class DailyProductivity
{
	public HourlyProductivity[] HourlyProductivities { get; }

	public HourlyProductivity CurrentProductivity { get; }

	public int CurrentHour { get; private set; }

	public DailyProductivity(HourlyProductivity[] hourlyProductivities, HourlyProductivity currentProductivity)
	{
		HourlyProductivities = hourlyProductivities;
		CurrentProductivity = currentProductivity;
	}

	public static DailyProductivity CreateDefault()
	{
		HourlyProductivity[] array = new HourlyProductivity[24];
		array.Fill(HourlyProductivity.CreateDefault);
		return new DailyProductivity(array, HourlyProductivity.CreateDefault());
	}

	public float CalculateProductivity()
	{
		float num = 0f;
		int num2 = 0;
		for (int i = 0; i < HourlyProductivities.Length; i++)
		{
			if (HourlyProductivities[i].WasWorkingHour)
			{
				num += HourlyProductivities[i].Productivity;
				num2++;
			}
		}
		if (num2 != 0)
		{
			return num / (float)num2;
		}
		return 0f;
	}

	public void UpdateAndSetCurrentHour(int currentHour)
	{
		HourlyProductivities[CurrentHour].CopyValuesFrom(CurrentProductivity);
		CurrentProductivity.Reset();
		SetCurrentHour(currentHour);
	}

	public void SetCurrentHour(int currentHour)
	{
		CurrentHour = currentHour;
	}

	public void AddSample(int maxWorkPotential, int actualWorkPerformed)
	{
		CurrentProductivity.AddSample(maxWorkPotential, actualWorkPerformed);
	}
}

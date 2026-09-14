namespace Timberborn.Workshops;

public class HourlyProductivity
{
	public int MaxWorkPotential { get; private set; }

	public int ActualWorkPerformed { get; private set; }

	public bool WasWorkingHour { get; private set; }

	public float Productivity
	{
		get
		{
			if (MaxWorkPotential != 0)
			{
				return (float)ActualWorkPerformed / (float)MaxWorkPotential;
			}
			return 0f;
		}
	}

	public HourlyProductivity(int maxWorkPotential, int actualWorkPerformed, bool wasWorkingHour)
	{
		MaxWorkPotential = maxWorkPotential;
		ActualWorkPerformed = actualWorkPerformed;
		WasWorkingHour = wasWorkingHour;
	}

	public static HourlyProductivity CreateDefault()
	{
		return new HourlyProductivity(0, 0, wasWorkingHour: false);
	}

	public void Reset()
	{
		MaxWorkPotential = 0;
		ActualWorkPerformed = 0;
		WasWorkingHour = false;
	}

	public void AddSample(int maxWorkPotential, int actualWorkPerformed)
	{
		MaxWorkPotential += maxWorkPotential;
		ActualWorkPerformed += actualWorkPerformed;
		WasWorkingHour = true;
	}

	public void CopyValuesFrom(HourlyProductivity otherProductivity)
	{
		MaxWorkPotential = otherProductivity.MaxWorkPotential;
		ActualWorkPerformed = otherProductivity.ActualWorkPerformed;
		WasWorkingHour = otherProductivity.WasWorkingHour;
	}
}

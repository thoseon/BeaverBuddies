namespace Timberborn.GameDistricts;

public class CitizenAssignedEventArgs
{
	public Citizen Citizen { get; }

	public CitizenAssignedEventArgs(Citizen citizen)
	{
		Citizen = citizen;
	}
}

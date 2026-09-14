namespace Timberborn.GameDistricts;

public class CitizenUnassignedEventArgs
{
	public Citizen Citizen { get; }

	public CitizenUnassignedEventArgs(Citizen citizen)
	{
		Citizen = citizen;
	}
}

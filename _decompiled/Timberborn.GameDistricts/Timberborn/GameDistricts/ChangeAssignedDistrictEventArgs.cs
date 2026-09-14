namespace Timberborn.GameDistricts;

public readonly struct ChangeAssignedDistrictEventArgs
{
	public DistrictCenter PreviousDistrict { get; }

	public DistrictCenter CurrentDistrict { get; }

	public ChangeAssignedDistrictEventArgs(DistrictCenter previousDistrict, DistrictCenter currentDistrict)
	{
		PreviousDistrict = previousDistrict;
		CurrentDistrict = currentDistrict;
	}
}

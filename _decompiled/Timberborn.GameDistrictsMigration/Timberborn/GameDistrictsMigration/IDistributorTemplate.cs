using Timberborn.GameDistricts;

namespace Timberborn.GameDistrictsMigration;

public interface IDistributorTemplate
{
	string ComponentName { get; }

	int Current { get; }

	void MigrateTo(DistrictCenter target, int amount);
}

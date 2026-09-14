using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Bots;
using Timberborn.GameDistricts;

namespace Timberborn.GameDistrictsMigration;

public class BotsDistributorTemplate : BaseComponent, IAwakableComponent, IDistributorTemplate
{
	private readonly MigrationService _migrationService;

	private DistrictCenter _districtCenter;

	public string ComponentName => "BotsDistributor";

	public int Current => _districtCenter.DistrictPopulation.NumberOfBots;

	public BotsDistributorTemplate(MigrationService migrationService)
	{
		_migrationService = migrationService;
	}

	public void Awake()
	{
		_districtCenter = GetComponent<DistrictCenter>();
	}

	public void MigrateTo(DistrictCenter target, int amount)
	{
		IOrderedEnumerable<Bot> charactersToMove = _districtCenter.DistrictPopulation.Bots.OrderBy(_migrationService.RefusesWork).ThenBy(_migrationService.IsEmployed).ThenByDescending(_migrationService.GetDayOfBirth);
		_migrationService.Migrate(charactersToMove, target, amount);
	}
}

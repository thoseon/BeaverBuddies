using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Beavers;
using Timberborn.GameDistricts;
using Timberborn.PopulationStatisticsSystem;

namespace Timberborn.GameDistrictsMigration;

public class AdultsDistributorTemplate : BaseComponent, IAwakableComponent, IDistributorTemplate
{
	private readonly MigrationService _migrationService;

	private DistrictCenter _districtCenter;

	private IContaminationStatisticsProvider _districtContaminationStatisticProvider;

	public string ComponentName => "AdultsDistributor";

	public int Current => _districtCenter.DistrictPopulation.NumberOfAdults - _districtContaminationStatisticProvider.GetContaminationStatistics().ContaminatedAdults;

	public AdultsDistributorTemplate(MigrationService migrationService)
	{
		_migrationService = migrationService;
	}

	public void Awake()
	{
		_districtCenter = GetComponent<DistrictCenter>();
		_districtContaminationStatisticProvider = _districtCenter.GetComponent<IContaminationStatisticsProvider>();
	}

	public void MigrateTo(DistrictCenter target, int amount)
	{
		IOrderedEnumerable<Beaver> charactersToMove = _districtCenter.DistrictPopulation.Adults.Where(_migrationService.IsNotContaminated).OrderBy(_migrationService.RefusesWork).ThenBy(_migrationService.IsEmployed)
			.ThenBy(_migrationService.HasHome)
			.ThenByDescending(_migrationService.GetDayOfBirth);
		_migrationService.Migrate(charactersToMove, target, amount);
	}
}

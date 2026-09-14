using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Beavers;
using Timberborn.GameDistricts;
using Timberborn.PopulationStatisticsSystem;

namespace Timberborn.GameDistrictsMigration;

public class ChildrenDistributorTemplate : BaseComponent, IAwakableComponent, IDistributorTemplate
{
	private readonly MigrationService _migrationService;

	private DistrictCenter _districtCenter;

	private IContaminationStatisticsProvider _districtContaminationStatisticProvider;

	public string ComponentName => "ChildrenDistributor";

	public int Current => _districtCenter.DistrictPopulation.NumberOfChildren - _districtContaminationStatisticProvider.GetContaminationStatistics().ContaminatedChildren;

	public ChildrenDistributorTemplate(MigrationService migrationService)
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
		IOrderedEnumerable<Beaver> charactersToMove = _districtCenter.DistrictPopulation.Children.Where(_migrationService.IsNotContaminated).OrderBy(_migrationService.HasHome).ThenByDescending(_migrationService.GetDayOfBirth);
		_migrationService.Migrate(charactersToMove, target, amount);
	}
}

using Timberborn.BeaverContaminationSystem;
using Timberborn.DwellingSystem;
using Timberborn.GameDistricts;
using Timberborn.PopulationStatisticsSystem;
using Timberborn.PopulationWorkStatistics;
using Timberborn.WorkerTypesUI;

namespace Timberborn.Population;

public class PopulationDataCollector
{
	public bool CollectData(int numberOfAdults, int numberOfChildren, int numberOfBots, IWorkRefusingStatisticsProvider workRefusingStatisticsProvider, IDwellingStatisticsProvider dwellingStatisticsProvider, IEmploymentStatisticsProvider employmentStatisticsProvider, IContaminationStatisticsProvider contaminationStatisticsProvider, PopulationData populationData)
	{
		WorkforceData workforceData = GetWorkforceData(workRefusingStatisticsProvider.GetWorkRefusingStatistics(WorkerTypeHelper.BeaverWorkerType));
		WorkforceData workforceData2 = GetWorkforceData(workRefusingStatisticsProvider.GetWorkRefusingStatistics(WorkerTypeHelper.BotWorkerType));
		BeaverContaminationStatistics contaminationStatistics = contaminationStatisticsProvider.GetContaminationStatistics();
		ContaminationData contaminationData = GetContaminationData(contaminationStatistics);
		BedData bedData = GetBedData(numberOfAdults + numberOfChildren, dwellingStatisticsProvider);
		EmploymentStatistics employmentStatistics = employmentStatisticsProvider.GetEmploymentStatistics(WorkerTypeHelper.BeaverWorkerType);
		WorkplaceData workplaceData = GetWorkplaceData(workforceData.Employable, employmentStatistics);
		EmploymentStatistics employmentStatistics2 = employmentStatisticsProvider.GetEmploymentStatistics(WorkerTypeHelper.BotWorkerType);
		WorkplaceData workplaceData2 = GetWorkplaceData(workforceData2.Employable, employmentStatistics2);
		if (populationData.NumberOfAdults != numberOfAdults || populationData.NumberOfChildren != numberOfChildren || populationData.NumberOfBots != numberOfBots || populationData.BeaverWorkforceData != workforceData || populationData.BotWorkforceData != workforceData2 || populationData.BedData != bedData || populationData.BeaverWorkplaceData != workplaceData || populationData.BotWorkplaceData != workplaceData2 || populationData.ContaminationData != contaminationData)
		{
			populationData.Update(numberOfAdults, numberOfChildren, numberOfBots, workforceData, workforceData2, bedData, workplaceData, workplaceData2, contaminationData);
			return true;
		}
		return false;
	}

	public bool CollectData(DistrictCenter districtCenter, PopulationData populationData)
	{
		return CollectData(districtCenter.DistrictPopulation.NumberOfAdults, districtCenter.DistrictPopulation.NumberOfChildren, districtCenter.DistrictPopulation.NumberOfBots, districtCenter.GetComponent<DistrictWorkRefusingStatisticsProvider>(), districtCenter.GetComponent<DistrictDwellingStatisticsProvider>(), districtCenter.GetComponent<DistrictEmploymentStatisticsProvider>(), districtCenter.GetComponent<DistrictBeaverContaminationStatisticsProvider>(), populationData);
	}

	private static WorkforceData GetWorkforceData(WorkRefusingStatistics workRefusingStatistics)
	{
		return new WorkforceData(workRefusingStatistics.NotRefusingWorkers, workRefusingStatistics.RefusingWorkers);
	}

	private static BedData GetBedData(int numberOfDwellers, IDwellingStatisticsProvider dwellingStatisticsProvider)
	{
		DwellingStatistics dwellingStatistics = dwellingStatisticsProvider.GetDwellingStatistics();
		int homeless = numberOfDwellers - dwellingStatistics.OccupiedBeds;
		return new BedData(dwellingStatistics.OccupiedBeds, dwellingStatistics.FreeBeds, homeless);
	}

	private static WorkplaceData GetWorkplaceData(int numberOfWorkers, EmploymentStatistics employmentStatistics)
	{
		int unemployed = numberOfWorkers - employmentStatistics.EmployedWorkers;
		return new WorkplaceData(employmentStatistics.EmployedWorkers, employmentStatistics.Vacancies, unemployed);
	}

	private ContaminationData GetContaminationData(BeaverContaminationStatistics beaverContaminationStatistics)
	{
		return new ContaminationData(beaverContaminationStatistics.ContaminatedAdults, beaverContaminationStatistics.ContaminatedChildren);
	}
}

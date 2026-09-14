using Timberborn.GameDistricts;

namespace Timberborn.GameDistrictsMigration;

public class MigrationNeighbours
{
	private readonly DistrictConnections _districtConnections;

	public MigrationNeighbours(DistrictConnections districtConnections)
	{
		_districtConnections = districtConnections;
	}

	public PopulationDistributor GetHighestSpareNeighbour(PopulationDistributor populationDistributor)
	{
		PopulationDistributor populationDistributor2 = null;
		foreach (DistrictCenter item in _districtConnections.GetDistrictsConnectedWith(populationDistributor.DistrictCenter))
		{
			PopulationDistributor otherDistrictPopulationDistributor = populationDistributor.GetOtherDistrictPopulationDistributor(item);
			if (otherDistrictPopulationDistributor.CanEmigrate && (populationDistributor2 == null || populationDistributor2.Spare < otherDistrictPopulationDistributor.Spare))
			{
				populationDistributor2 = otherDistrictPopulationDistributor;
			}
		}
		return populationDistributor2;
	}

	public PopulationDistributor GetLowestSpareNeighbour(PopulationDistributor populationDistributor)
	{
		PopulationDistributor populationDistributor2 = null;
		foreach (DistrictCenter item in _districtConnections.GetDistrictsConnectedWith(populationDistributor.DistrictCenter))
		{
			PopulationDistributor otherDistrictPopulationDistributor = populationDistributor.GetOtherDistrictPopulationDistributor(item);
			if (otherDistrictPopulationDistributor.CanImmigrate && (populationDistributor2 == null || populationDistributor2.Spare > otherDistrictPopulationDistributor.Spare))
			{
				populationDistributor2 = otherDistrictPopulationDistributor;
			}
		}
		return populationDistributor2;
	}
}

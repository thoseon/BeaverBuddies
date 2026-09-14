namespace Timberborn.Population;

public class PopulationData
{
	public int NumberOfAdults { get; private set; }

	public int NumberOfChildren { get; private set; }

	public int NumberOfBots { get; private set; }

	public WorkforceData BeaverWorkforceData { get; private set; }

	public WorkforceData BotWorkforceData { get; private set; }

	public BedData BedData { get; private set; }

	public WorkplaceData BeaverWorkplaceData { get; private set; }

	public WorkplaceData BotWorkplaceData { get; private set; }

	public ContaminationData ContaminationData { get; private set; }

	public int NumberOfBeavers => NumberOfAdults + NumberOfChildren;

	public int NumberOfHealthyAdults => NumberOfAdults - ContaminationData.ContaminatedAdults;

	public int NumberOfHealthyChildren => NumberOfChildren - ContaminationData.ContaminatedChildren;

	public int TotalPopulation => NumberOfBeavers + NumberOfBots;

	public void Update(int numberOfAdults, int numberOfChildren, int numberOfBots, WorkforceData beaverWorkforceData, WorkforceData botWorkforceData, BedData bedData, WorkplaceData beaverWorkplaceData, WorkplaceData botWorkplaceData, ContaminationData contaminationData)
	{
		NumberOfAdults = numberOfAdults;
		NumberOfChildren = numberOfChildren;
		NumberOfBots = numberOfBots;
		BeaverWorkforceData = beaverWorkforceData;
		BotWorkforceData = botWorkforceData;
		BedData = bedData;
		BeaverWorkplaceData = beaverWorkplaceData;
		BotWorkplaceData = botWorkplaceData;
		ContaminationData = contaminationData;
	}

	public void CopyFrom(PopulationData other)
	{
		NumberOfAdults = other.NumberOfAdults;
		NumberOfChildren = other.NumberOfChildren;
		NumberOfBots = other.NumberOfBots;
		BeaverWorkforceData = other.BeaverWorkforceData;
		BotWorkforceData = other.BotWorkforceData;
		BedData = other.BedData;
		BeaverWorkplaceData = other.BeaverWorkplaceData;
		BotWorkplaceData = other.BotWorkplaceData;
		ContaminationData = other.ContaminationData;
	}
}

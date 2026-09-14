using System;

namespace Timberborn.PopulationStatisticsSampling;

public struct PopulationSample
{
	public int Day { get; }

	public int Cycle { get; }

	public int Adults { get; }

	public int Children { get; }

	public int Contaminated { get; }

	public int Bots { get; }

	public int Wellbeing { get; private set; }

	public int Births { get; }

	public int Deaths { get; }

	public int BotCreations { get; }

	public int BotDestructions { get; }

	public int OccupiedBeds { get; }

	public int FreeBeds { get; }

	public int Homeless { get; }

	public int EmployedBeavers { get; }

	public int FreeWorkSlotsBeavers { get; }

	public int UnemployedBeavers { get; }

	public int UnemployableBeavers { get; }

	public int EmployedBots { get; }

	public int FreeWorkSlotsBots { get; }

	public int UnemployedBots { get; }

	public int UnemployableBots { get; }

	public PopulationSample(int day, int cycle, int adults, int children, int contaminated, int bots, int wellbeing, int births, int deaths, int botCreations, int botDestructions, int occupiedBeds, int freeBeds, int homeless, int employedBeavers, int freeWorkSlotsBeavers, int unemployedBeavers, int unemployableBeavers, int employedBots, int freeWorkSlotsBots, int unemployedBots, int unemployableBots)
	{
		Day = day;
		Cycle = cycle;
		Adults = adults;
		Children = children;
		Contaminated = contaminated;
		Bots = bots;
		Wellbeing = wellbeing;
		Births = births;
		Deaths = deaths;
		BotCreations = botCreations;
		BotDestructions = botDestructions;
		OccupiedBeds = occupiedBeds;
		FreeBeds = freeBeds;
		Homeless = homeless;
		EmployedBeavers = employedBeavers;
		FreeWorkSlotsBeavers = freeWorkSlotsBeavers;
		UnemployedBeavers = unemployedBeavers;
		UnemployableBeavers = unemployableBeavers;
		EmployedBots = employedBots;
		FreeWorkSlotsBots = freeWorkSlotsBots;
		UnemployedBots = unemployedBots;
		UnemployableBots = unemployableBots;
	}

	public int GetValueById(string stringId)
	{
		return stringId switch
		{
			"TotalPopulation" => Adults + Children + Bots, 
			"Adults" => Adults, 
			"Children" => Children, 
			"Contaminated" => Contaminated, 
			"Bots" => Bots, 
			"Wellbeing" => Wellbeing, 
			"Births" => Births, 
			"Deaths" => Deaths, 
			"BotCreations" => BotCreations, 
			"BotDestructions" => BotDestructions, 
			"TotalBeds" => OccupiedBeds + FreeBeds, 
			"OccupiedBeds" => OccupiedBeds, 
			"FreeBeds" => FreeBeds, 
			"Homeless" => Homeless, 
			"EmployedBeavers" => EmployedBeavers, 
			"FreeWorkSlotsBeavers" => FreeWorkSlotsBeavers, 
			"UnemployedBeavers" => UnemployedBeavers, 
			"UnemployableBeavers" => UnemployableBeavers, 
			"EmployedBots" => EmployedBots, 
			"FreeWorkSlotsBots" => FreeWorkSlotsBots, 
			"UnemployedBots" => UnemployedBots, 
			"UnemployableBots" => UnemployableBots, 
			_ => throw new ArgumentException("Unknown stringId: " + stringId), 
		};
	}

	public void SetWellbeing(int wellbeing)
	{
		Wellbeing = wellbeing;
	}

	public static PopulationSample operator +(PopulationSample left, PopulationSample right)
	{
		return new PopulationSample((left.Day == 0) ? right.Day : left.Day, (left.Cycle == 0) ? right.Cycle : left.Cycle, left.Adults + right.Adults, left.Children + right.Children, left.Contaminated + right.Contaminated, left.Bots + right.Bots, left.Wellbeing + right.Wellbeing, left.Births + right.Births, left.Deaths + right.Deaths, left.BotCreations + right.BotCreations, left.BotDestructions + right.BotDestructions, left.OccupiedBeds + right.OccupiedBeds, left.FreeBeds + right.FreeBeds, left.Homeless + right.Homeless, left.EmployedBeavers + right.EmployedBeavers, left.FreeWorkSlotsBeavers + right.FreeWorkSlotsBeavers, left.UnemployedBeavers + right.UnemployedBeavers, left.UnemployableBeavers + right.UnemployableBeavers, left.EmployedBots + right.EmployedBots, left.FreeWorkSlotsBots + right.FreeWorkSlotsBots, left.UnemployedBots + right.UnemployedBots, left.UnemployableBots + right.UnemployableBots);
	}
}

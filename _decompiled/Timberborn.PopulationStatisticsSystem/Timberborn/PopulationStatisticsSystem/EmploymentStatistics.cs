using System;

namespace Timberborn.PopulationStatisticsSystem;

public readonly struct EmploymentStatistics
{
	public int EmployedWorkers { get; }

	public int Vacancies { get; }

	public string WorkerType { get; }

	public EmploymentStatistics(int employedWorkers, int vacancies, string workerType)
	{
		EmployedWorkers = employedWorkers;
		Vacancies = vacancies;
		WorkerType = workerType;
	}

	public static EmploymentStatistics operator +(EmploymentStatistics left, EmploymentStatistics right)
	{
		if (left.WorkerType != right.WorkerType)
		{
			throw new Exception("Cannot add EmploymentStatistics with different WorkerType");
		}
		return new EmploymentStatistics(left.EmployedWorkers + right.EmployedWorkers, left.Vacancies + right.Vacancies, left.WorkerType);
	}

	public static EmploymentStatistics operator -(EmploymentStatistics left, EmploymentStatistics right)
	{
		if (left.WorkerType != right.WorkerType)
		{
			throw new Exception("Cannot subtract EmploymentStatistics with different WorkerType");
		}
		return new EmploymentStatistics(left.EmployedWorkers - right.EmployedWorkers, left.Vacancies - right.Vacancies, left.WorkerType);
	}
}

using System;

namespace Timberborn.WorkSystem;

public readonly struct UnlockableWorkerType : IEquatable<UnlockableWorkerType>
{
	public string WorkplaceTemplateName { get; }

	public string WorkerType { get; }

	public UnlockableWorkerType(string workplaceTemplateName, string workerType)
	{
		WorkplaceTemplateName = workplaceTemplateName;
		WorkerType = workerType;
	}

	public bool Equals(UnlockableWorkerType other)
	{
		if (WorkplaceTemplateName == other.WorkplaceTemplateName)
		{
			return WorkerType == other.WorkerType;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is UnlockableWorkerType other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(WorkplaceTemplateName, WorkerType);
	}

	public static bool operator ==(UnlockableWorkerType left, UnlockableWorkerType right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(UnlockableWorkerType left, UnlockableWorkerType right)
	{
		return !left.Equals(right);
	}
}

namespace Timberborn.PrioritySystem;

public interface IPrioritizable
{
	Priority Priority { get; }

	void SetPriority(Priority priority);
}

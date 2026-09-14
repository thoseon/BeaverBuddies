using System.Collections.Immutable;

namespace Timberborn.StatusSystem;

public interface IStatusAggregator
{
	ImmutableArray<StatusInstance> GetVisibleStatuses(string alertDescription);
}

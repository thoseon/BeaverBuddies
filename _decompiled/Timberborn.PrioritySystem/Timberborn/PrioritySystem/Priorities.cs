using System;
using System.Collections.Immutable;
using System.Linq;

namespace Timberborn.PrioritySystem;

public static class Priorities
{
	public static readonly ImmutableArray<Priority> Ascending = Enum.GetValues(typeof(Priority)).Cast<Priority>().ToImmutableArray();

	public static readonly ImmutableArray<Priority> Descending = Ascending.OrderByDescending((Priority priority) => priority).ToImmutableArray();
}

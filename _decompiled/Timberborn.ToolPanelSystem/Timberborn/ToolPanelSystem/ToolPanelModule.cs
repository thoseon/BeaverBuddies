using System.Collections.Generic;
using System.Collections.Immutable;

namespace Timberborn.ToolPanelSystem;

public class ToolPanelModule
{
	public class Builder
	{
		private readonly List<OrderedToolFragment> _toolFragments = new List<OrderedToolFragment>();

		public void AddFragment(IToolFragment toolFragment, int order)
		{
			_toolFragments.Add(new OrderedToolFragment(toolFragment, order));
		}

		public ToolPanelModule Build()
		{
			return new ToolPanelModule(_toolFragments);
		}
	}

	public ImmutableArray<OrderedToolFragment> ToolFragments { get; }

	private ToolPanelModule(IEnumerable<OrderedToolFragment> toolFragments)
	{
		ToolFragments = toolFragments.ToImmutableArray();
	}
}

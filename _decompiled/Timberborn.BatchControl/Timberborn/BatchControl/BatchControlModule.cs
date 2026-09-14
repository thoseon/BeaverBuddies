using System.Collections.Frozen;
using System.Collections.Generic;

namespace Timberborn.BatchControl;

public class BatchControlModule
{
	public class Builder
	{
		private readonly Dictionary<int, BatchControlTab> _tabs = new Dictionary<int, BatchControlTab>();

		public void AddTab(BatchControlTab tab, int order)
		{
			_tabs.Add(order, tab);
		}

		public BatchControlModule Build()
		{
			return new BatchControlModule(_tabs);
		}
	}

	public FrozenDictionary<int, BatchControlTab> Tabs { get; }

	private BatchControlModule(Dictionary<int, BatchControlTab> tabs)
	{
		Tabs = tabs.ToFrozenDictionary();
	}
}

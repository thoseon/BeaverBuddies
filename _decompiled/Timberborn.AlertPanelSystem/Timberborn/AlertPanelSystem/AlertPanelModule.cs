using System.Collections.Frozen;
using System.Collections.Generic;

namespace Timberborn.AlertPanelSystem;

public class AlertPanelModule
{
	public class Builder
	{
		private readonly Dictionary<int, IAlertFragment> _alertFragments = new Dictionary<int, IAlertFragment>();

		public void AddAlertFragment(IAlertFragment alertFragment, int order)
		{
			_alertFragments.Add(order, alertFragment);
		}

		public AlertPanelModule Build()
		{
			return new AlertPanelModule(_alertFragments);
		}
	}

	public FrozenDictionary<int, IAlertFragment> AlertFragments { get; }

	private AlertPanelModule(Dictionary<int, IAlertFragment> panels)
	{
		AlertFragments = panels.ToFrozenDictionary();
	}
}

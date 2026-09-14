using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

public class PopulationWellbeingCounterGroup
{
	public VisualElement Root { get; }

	public List<PopulationWellbeingCounter> Counters { get; }

	public bool HasCounters => Counters.Count > 0;

	public PopulationWellbeingCounterGroup(VisualElement root, IEnumerable<PopulationWellbeingCounter> counters)
	{
		Root = root;
		Counters = counters.ToList();
	}
}

using System.Linq;
using System.Text;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.PopulationStatisticsSampling;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.PopulationStatisticsBatchControl;

internal class PopulationStatisticsTooltipRegistrar
{
	private static readonly string TimestampLocKey = "Weather.CycleAndDayLong";

	private readonly ILoc _loc;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly StringBuilder _text = new StringBuilder();

	public PopulationStatisticsTooltipRegistrar(ILoc loc, VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar)
	{
		_loc = loc;
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public void Register(PopulationSampleHistory populationSampleHistory, ReadOnlyList<PopulationGraphDefinition> graphDefinitions, VisualElement root, int index)
	{
		_tooltipRegistrar.Register(root, () => TooltipContent.CreateInstant(() => Create(populationSampleHistory, graphDefinitions, index)));
	}

	private VisualElement Create(PopulationSampleHistory populationSampleHistory, ReadOnlyList<PopulationGraphDefinition> graphDefinitions, int index)
	{
		string elementName = "Game/BatchControl/PopulationStatisticsTooltip";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		int count = graphDefinitions[0].GraphDefinition.DataPoints.Count;
		int count2 = populationSampleHistory.PopulationSamples.Count;
		int num = count2 - count + index;
		if (num >= 0 && num < count2)
		{
			PopulationSample populationSample = populationSampleHistory.PopulationSamples[num];
			string text = _loc.T(TimestampLocKey, populationSample.Cycle, populationSample.Day);
			visualElement.Q<Label>("Title").text = text ?? "";
			foreach (PopulationGraphDefinition item in graphDefinitions)
			{
				float num2 = item.GraphDefinition.DataPoints.ElementAtOrDefault(index);
				_text.AppendLine($"{item.GetDisplayName()}: {num2}");
			}
			visualElement.Q<Label>("Content").text = _text.ToStringWithoutNewLineEndAndClean();
			return visualElement;
		}
		return null;
	}
}

using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.SettlementStatistics;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.GameWonderCompletionUI;

public class StatisticItemFactory
{
	private readonly ILoc _loc;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly IncrementalStatisticCollector _incrementalStatisticCollector;

	public StatisticItemFactory(ILoc loc, VisualElementLoader visualElementLoader, IncrementalStatisticCollector incrementalStatisticCollector)
	{
		_loc = loc;
		_visualElementLoader = visualElementLoader;
		_incrementalStatisticCollector = incrementalStatisticCollector;
	}

	public VisualElement Create(string statisticId)
	{
		return Create(statisticId, alwaysVisible: true);
	}

	public VisualElement CreateIfHasValue(string statisticId)
	{
		return Create(statisticId, alwaysVisible: false);
	}

	private VisualElement Create(string statisticId, bool alwaysVisible)
	{
		int orDefault = _incrementalStatisticCollector.GetOrDefault(statisticId);
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/WonderCompletion/StatisticItem");
		visualElement.Q<Label>("Name").text = _loc.T("WonderCompletion.Statistic." + statisticId);
		visualElement.Q<Label>("Value").text = NumberFormatter.FormatFullNumber(orDefault);
		visualElement.ToggleDisplayStyle(alwaysVisible || orDefault > 0);
		return visualElement;
	}
}

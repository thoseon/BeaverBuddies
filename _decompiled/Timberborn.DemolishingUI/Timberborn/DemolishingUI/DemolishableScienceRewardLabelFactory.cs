using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.DemolishingUI;

public class DemolishableScienceRewardLabelFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	public DemolishableScienceRewardLabelFactory(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public DemolishableScienceRewardLabel Create()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/EntityPanel/DemolishableScienceReward");
		return new DemolishableScienceRewardLabel(visualElement, visualElement.Q<Label>("SciencePoints"));
	}
}

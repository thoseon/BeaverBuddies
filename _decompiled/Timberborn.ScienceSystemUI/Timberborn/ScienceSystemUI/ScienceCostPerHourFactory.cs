using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.ScienceSystemUI;

public class ScienceCostPerHourFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	public ScienceCostPerHourFactory(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public ScienceCostPerHour Create()
	{
		string elementName = "Game/EntityPanel/ScienceCostPerHour";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		Label scienceCostValue = visualElement.Q<Label>("ScienceCostValue");
		return new ScienceCostPerHour(visualElement, scienceCostValue);
	}
}

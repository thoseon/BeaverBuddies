using UnityEngine.UIElements;

namespace Timberborn.ScienceSystemUI;

public class ScienceCostPerHour
{
	private readonly Label _scienceCostValue;

	public VisualElement Root { get; }

	public ScienceCostPerHour(VisualElement root, Label scienceCostValue)
	{
		Root = root;
		_scienceCostValue = scienceCostValue;
	}

	public void UpdateCost(int scienceCost)
	{
		_scienceCostValue.text = scienceCost.ToString();
	}
}

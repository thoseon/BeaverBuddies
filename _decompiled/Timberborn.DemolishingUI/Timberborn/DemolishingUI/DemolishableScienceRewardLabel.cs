using Timberborn.CoreUI;
using Timberborn.Demolishing;
using UnityEngine.UIElements;

namespace Timberborn.DemolishingUI;

public class DemolishableScienceRewardLabel
{
	private readonly Label _points;

	public VisualElement Root { get; }

	public DemolishableScienceRewardLabel(VisualElement root, Label points)
	{
		Root = root;
		_points = points;
	}

	public void Show(DemolishableScienceRewardSpec spec)
	{
		bool flag = spec != null;
		if (flag)
		{
			_points.text = spec.SciencePoints.ToString();
		}
		Root.ToggleDisplayStyle(flag);
	}
}

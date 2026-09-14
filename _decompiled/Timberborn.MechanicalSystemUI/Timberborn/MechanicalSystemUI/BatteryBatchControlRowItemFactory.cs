using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.MechanicalSystem;
using UnityEngine.UIElements;

namespace Timberborn.MechanicalSystemUI;

public class BatteryBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	public BatteryBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		MechanicalNode component = entity.GetComponent<MechanicalNode>();
		if (component != null && component.IsBattery)
		{
			string elementName = "Game/BatchControl/BatteryBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			Timberborn.CoreUI.ProgressBar progressBar = visualElement.Q<Timberborn.CoreUI.ProgressBar>("ProgressBar");
			Label chargeLabel = visualElement.Q<Label>("Charge");
			return new BatteryBatchControlRowItem(_loc, visualElement, progressBar, chargeLabel, component);
		}
		return null;
	}
}

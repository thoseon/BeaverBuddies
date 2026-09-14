using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.MechanicalSystem;
using UnityEngine.UIElements;

namespace Timberborn.MechanicalSystemUI;

public class MechanicalBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly MechanicalNodeTextFormatter _mechanicalNodeTextFormatter;

	internal MechanicalBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ILoc loc, MechanicalNodeTextFormatter mechanicalNodeTextFormatter)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_mechanicalNodeTextFormatter = mechanicalNodeTextFormatter;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		MechanicalNode component = entity.GetComponent<MechanicalNode>();
		if (component != null && component.Enabled)
		{
			string elementName = "Game/BatchControl/MechanicalBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			Label label = visualElement.Q<Label>("MechanicalBatchControlRowItem");
			return new MechanicalBatchControlRowItem(_mechanicalNodeTextFormatter, visualElement, label, component);
		}
		return null;
	}

	public IBatchControlRowItem Create(MechanicalGraph mechanicalGraph)
	{
		string elementName = "Game/BatchControl/MechanicalHeaderBatchControlRowItem";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		Label label = visualElement.Q<Label>("MechanicalHeaderBatchControlRowItem");
		return new MechanicalHeaderBatchControlRowItem(_loc, visualElement, label, mechanicalGraph);
	}
}

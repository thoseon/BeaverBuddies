using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.AutomationUI;

public class AutomatableBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly AutomationStateIconBuilder _automationStateIconBuilder;

	public AutomatableBatchControlRowItemFactory(VisualElementLoader visualElementLoader, AutomationStateIconBuilder automationStateIconBuilder)
	{
		_visualElementLoader = visualElementLoader;
		_automationStateIconBuilder = automationStateIconBuilder;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		Automatable automatable = entity.GetComponent<Automatable>();
		if (automatable != null)
		{
			string elementName = "Game/BatchControl/AutomatableBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			AutomationStateIcon automationStateIcon = _automationStateIconBuilder.Create(visualElement.Q<Image>("StateIcon"), () => automatable.Input).SetClickableIcon().Build();
			return new AutomatableBatchControlRowItem(visualElement, automatable, automationStateIcon);
		}
		return null;
	}
}

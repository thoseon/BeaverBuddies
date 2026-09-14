using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.TooltipSystem;
using Timberborn.WorkSystem;
using UnityEngine.UIElements;

namespace Timberborn.WorkSystemUI;

public class WorkplaceBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	public WorkplaceBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		Workplace workplace = entity.GetComponent<Workplace>();
		if (workplace != null)
		{
			string elementName = "Game/BatchControl/WorkplaceBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			Label label = visualElement.Q<Label>("Info");
			Button button = visualElement.Q<Button>("Decrease");
			button.RegisterCallback<ClickEvent>(delegate
			{
				workplace.DecreaseDesiredWorkers();
			});
			button.SetEnabled(workplace.MaxWorkers > 1);
			Button button2 = visualElement.Q<Button>("Increase");
			button2.RegisterCallback<ClickEvent>(delegate
			{
				workplace.IncreaseDesiredWorkers();
			});
			button2.SetEnabled(workplace.MaxWorkers > 1);
			WorkplaceDescriber component = workplace.GetComponent<WorkplaceDescriber>();
			_tooltipRegistrar.Register((VisualElement)label, (Func<string>)component.GetWorkersTooltip);
			_tooltipRegistrar.Register(visualElement.Q<VisualElement>("Workers"), (Func<string>)component.GetWorkersTooltip);
			return new WorkplaceBatchControlRowItem(visualElement, workplace, label, button2, button);
		}
		return null;
	}
}

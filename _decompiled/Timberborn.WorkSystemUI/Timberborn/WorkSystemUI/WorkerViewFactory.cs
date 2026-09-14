using Timberborn.CharactersUI;
using Timberborn.CoreUI;
using Timberborn.TooltipSystem;
using Timberborn.WorkSystem;
using Timberborn.WorkerTypesUI;
using UnityEngine.UIElements;

namespace Timberborn.WorkSystemUI;

internal class WorkerViewFactory
{
	private static readonly string VacantLocKey = "Work.Vacant";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly CharacterButtonFactory _characterButtonFactory;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly WorkerTypeHelper _workerTypeHelper;

	public WorkerViewFactory(VisualElementLoader visualElementLoader, CharacterButtonFactory characterButtonFactory, ITooltipRegistrar tooltipRegistrar, WorkerTypeHelper workerTypeHelper)
	{
		_visualElementLoader = visualElementLoader;
		_characterButtonFactory = characterButtonFactory;
		_tooltipRegistrar = tooltipRegistrar;
		_workerTypeHelper = workerTypeHelper;
	}

	public WorkerView Create(WorkplaceWorkerType workplaceWorkerType)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/EntityPanel/WorkerView");
		CharacterButton characterButton = _characterButtonFactory.Create(visualElement.Q<Button>("CharacterButton"));
		visualElement.Q<Button>("WorkerView").RegisterCallback<ClickEvent>(delegate
		{
			characterButton.ClickAction();
		});
		VisualElement visualElement2 = visualElement.Q<VisualElement>("VacantIcon");
		_tooltipRegistrar.RegisterLocalizable(visualElement2, VacantLocKey);
		return new WorkerView(_workerTypeHelper, visualElement, characterButton, visualElement.Q<Label>("Name"), visualElement2, workplaceWorkerType);
	}
}

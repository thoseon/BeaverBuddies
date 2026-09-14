using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using Timberborn.WorkerTypesUI;
using UnityEngine.UIElements;

namespace Timberborn.WorkSystemUI;

public class WorkerTypeToggleFactory
{
	private readonly SliderToggleFactory _sliderToggleFactory;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly WorkerTypeHelper _workerTypeHelper;

	private readonly WorkplaceUnlockingDialogService _workplaceUnlockingDialogService;

	private readonly ILoc _loc;

	public WorkerTypeToggleFactory(SliderToggleFactory sliderToggleFactory, VisualElementLoader visualElementLoader, WorkerTypeHelper workerTypeHelper, WorkplaceUnlockingDialogService workplaceUnlockingDialogService, ILoc loc)
	{
		_sliderToggleFactory = sliderToggleFactory;
		_visualElementLoader = visualElementLoader;
		_workerTypeHelper = workerTypeHelper;
		_workplaceUnlockingDialogService = workplaceUnlockingDialogService;
		_loc = loc;
	}

	public WorkerTypeToggle Create(VisualElement parent)
	{
		return CreateBindable(parent, null);
	}

	public WorkerTypeToggle CreateBindable(VisualElement parent, string toggleBindingKey)
	{
		WorkerTypeToggle workerTypeToggle = new WorkerTypeToggle(_sliderToggleFactory, _visualElementLoader, _workerTypeHelper, _workplaceUnlockingDialogService, _loc);
		workerTypeToggle.Initialize(parent, toggleBindingKey);
		return workerTypeToggle;
	}
}

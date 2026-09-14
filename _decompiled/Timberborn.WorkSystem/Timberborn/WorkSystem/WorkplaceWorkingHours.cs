using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.WorkerTypes;

namespace Timberborn.WorkSystem;

public class WorkplaceWorkingHours : BaseComponent, IInitializableEntity
{
	private readonly WorkingHoursManager _workingHoursManager;

	private readonly WorkerTypeService _workerTypeService;

	private bool _ignoreWorkingHours;

	public bool AreWorkingHours
	{
		get
		{
			if (!_ignoreWorkingHours)
			{
				return _workingHoursManager.AreWorkingHours;
			}
			return true;
		}
	}

	public WorkplaceWorkingHours(WorkingHoursManager workingHoursManager, WorkerTypeService workerTypeService)
	{
		_workingHoursManager = workingHoursManager;
		_workerTypeService = workerTypeService;
	}

	public void InitializeEntity()
	{
		WorkplaceWorkerType component = GetComponent<WorkplaceWorkerType>();
		component.WorkerTypeChanged += OnWorkerTypeChanged;
		UpdateIgnoringWorkingHours(component.WorkerType);
	}

	private void OnWorkerTypeChanged(object sender, WorkerTypeChangedEventArgs e)
	{
		UpdateIgnoringWorkingHours(e.CurrentWorkerType);
	}

	private void UpdateIgnoringWorkingHours(string workerType)
	{
		WorkerTypeSpec workerTypeSpec = _workerTypeService.GetWorkerTypeSpec(workerType);
		_ignoreWorkingHours = workerTypeSpec.IgnoresWorkingHours;
	}
}

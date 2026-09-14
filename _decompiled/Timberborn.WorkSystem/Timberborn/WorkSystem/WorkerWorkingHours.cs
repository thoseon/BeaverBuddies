using Timberborn.BaseComponentSystem;
using Timberborn.WorkerTypes;

namespace Timberborn.WorkSystem;

public class WorkerWorkingHours : BaseComponent, IAwakableComponent
{
	private readonly WorkerTypeService _workerTypeService;

	private readonly WorkingHoursManager _workingHoursManager;

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

	public WorkerWorkingHours(WorkerTypeService workerTypeService, WorkingHoursManager workingHoursManager)
	{
		_workerTypeService = workerTypeService;
		_workingHoursManager = workingHoursManager;
	}

	public void Awake()
	{
		string workerType = GetComponent<Worker>().WorkerType;
		WorkerTypeSpec workerTypeSpec = _workerTypeService.GetWorkerTypeSpec(workerType);
		_ignoreWorkingHours = workerTypeSpec.IgnoresWorkingHours;
	}
}

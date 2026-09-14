using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.WorkerTypes;

public class WorkerTypeService : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private readonly Dictionary<string, WorkerTypeSpec> _workerTypes = new Dictionary<string, WorkerTypeSpec>();

	public WorkerTypeService(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		foreach (WorkerTypeSpec spec in _specService.GetSpecs<WorkerTypeSpec>())
		{
			_workerTypes.Add(spec.Id, spec);
			foreach (string backwardCompatibleId in spec.BackwardCompatibleIds)
			{
				_workerTypes.Add(backwardCompatibleId, spec);
			}
		}
	}

	public WorkerTypeSpec GetWorkerTypeSpec(string workerType)
	{
		return _workerTypes[workerType];
	}

	public string GetWorkerType(string initialWorkerType)
	{
		return GetWorkerTypeSpec(initialWorkerType).Id;
	}
}

using System.Collections.Generic;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.GameFactionSystem;
using Timberborn.SingletonSystem;
using Timberborn.WorkSystem;

namespace Timberborn.WorkerOutfitSystem;

internal class WorkerOutfitService : ILoadableSingleton
{
	private readonly FactionService _factionService;

	private readonly ISpecService _specService;

	private Dictionary<int, WorkerOutfitSpec> _workerOutfitSpecs;

	public WorkerOutfitService(FactionService factionService, ISpecService specService)
	{
		_factionService = factionService;
		_specService = specService;
	}

	public void Load()
	{
		_workerOutfitSpecs = (from s in _specService.GetSpecs<WorkerOutfitSpec>()
			where s.FactionId == _factionService.Current.Id
			select s).ToDictionary((WorkerOutfitSpec s) => GetSpecKey(s.Id, s.WorkerType), (WorkerOutfitSpec s) => s);
	}

	public bool TryGetOutfitSpec(Worker worker, out WorkerOutfitSpec workerOutfitSpec)
	{
		workerOutfitSpec = null;
		WorkplaceWorkerOutfitSpec component = worker.Workplace.GetComponent<WorkplaceWorkerOutfitSpec>();
		if (component != null)
		{
			string workerOutfit = component.WorkerOutfit;
			if (!string.IsNullOrWhiteSpace(workerOutfit))
			{
				int specKey = GetSpecKey(workerOutfit, worker.WorkerType);
				return _workerOutfitSpecs.TryGetValue(specKey, out workerOutfitSpec);
			}
		}
		return false;
	}

	private static int GetSpecKey(string id, string workerType)
	{
		return (id.GetHashCode() * 397) ^ workerType.GetHashCode();
	}
}

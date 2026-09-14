using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.EnterableSystem;
using Timberborn.WalkingSystem;
using Timberborn.WorkSystem;

namespace Timberborn.Workshops;

public class WorkplaceWorkStarter : BaseComponent, IAwakableComponent
{
	private WalkInsideExecutor _walkInsideExecutor;

	private WorkExecutor _workExecutor;

	private Worker _worker;

	public void Awake()
	{
		_worker = GetComponent<Worker>();
		_walkInsideExecutor = GetComponent<WalkInsideExecutor>();
		_workExecutor = GetComponent<WorkExecutor>();
	}

	public Decision StartWorking()
	{
		Workplace workplace = _worker.Workplace;
		Enterable component = workplace.GetComponent<Enterable>();
		switch (_walkInsideExecutor.Launch(component))
		{
		case ExecutorStatus.Success:
			_workExecutor.Launch(0.25f);
			return Decision.ReleaseWhenFinished(_workExecutor);
		case ExecutorStatus.Failure:
			workplace.UnassignWorker(_worker);
			return Decision.ReleaseNextTick();
		case ExecutorStatus.Running:
			return Decision.ReleaseWhenFinished(_walkInsideExecutor);
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}

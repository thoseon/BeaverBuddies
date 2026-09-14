using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.EnterableSystem;
using Timberborn.Navigation;
using Timberborn.WalkingSystem;
using Timberborn.WorkSystem;

namespace Timberborn.ConstructionSites;

public class BuildBehavior : Behavior, IAwakableComponent, IJobBehavior
{
	private Builder _builder;

	private WalkToAccessibleExecutor _walkToAccessibleExecutor;

	private BuildExecutor _buildExecutor;

	private BehaviorAgent _behaviorAgent;

	private Enterer _enterer;

	public void Awake()
	{
		_builder = GetComponent<Builder>();
		_behaviorAgent = GetComponent<BehaviorAgent>();
		_enterer = GetComponent<Enterer>();
		_walkToAccessibleExecutor = GetComponent<WalkToAccessibleExecutor>();
		_buildExecutor = GetComponent<BuildExecutor>();
	}

	public Decision StartBuilding(ConstructionSite constructionSite)
	{
		_builder.Reserve(constructionSite);
		return Decide(_behaviorAgent);
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (_builder.HasReservedConstructionSite)
		{
			ConstructionSite reservedConstructionSite = _builder.ReservedConstructionSite;
			Accessible enabledComponent = reservedConstructionSite.GetEnabledComponent<Accessible>();
			Enterable enabledComponent2 = reservedConstructionSite.GetEnabledComponent<Enterable>();
			if ((bool)enabledComponent2 && _enterer.CurrentBuilding == enabledComponent2)
			{
				return BeginConstruction(reservedConstructionSite);
			}
			switch (_walkToAccessibleExecutor.Launch(enabledComponent))
			{
			case ExecutorStatus.Success:
				if ((bool)enabledComponent2 && enabledComponent2.CanEnter)
				{
					_enterer.Enter(enabledComponent2);
				}
				return BeginConstruction(reservedConstructionSite);
			case ExecutorStatus.Failure:
				_builder.Unreserve();
				return Decision.ReleaseNextTick();
			case ExecutorStatus.Running:
				return Decision.ReturnWhenFinished(_walkToAccessibleExecutor);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		return Decision.ReleaseNow();
	}

	private Decision BeginConstruction(ConstructionSite constructionSite)
	{
		if (_buildExecutor.Launch(constructionSite))
		{
			return Decision.ReleaseWhenFinished(_buildExecutor);
		}
		_builder.Unreserve();
		return Decision.ReleaseNextTick();
	}
}

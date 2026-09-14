using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.Beavers;

public class ChildRootBehavior : RootBehavior, IAwakableComponent, IPersistentEntity
{
	private static readonly ComponentKey ChildRootBehaviorKey = new ComponentKey("ChildRootBehavior");

	private static readonly PropertyKey<bool> WaitedAfterBirthKey = new PropertyKey<bool>("WaitedAfterBirth");

	private Child _child;

	private WaitExecutor _waitExecutor;

	private bool _waitedAfterBirth;

	public void Awake()
	{
		_child = GetComponent<Child>();
		_waitExecutor = GetComponent<WaitExecutor>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (!_waitedAfterBirth && _child.IsNewborn)
		{
			_waitExecutor.LaunchForIdleTime();
			_waitedAfterBirth = true;
			return Decision.ReleaseWhenFinished(_waitExecutor);
		}
		if (!_child.GrowUpIfItIsTime())
		{
			return Decision.ReleaseNow();
		}
		return Decision.ReturnNextTick();
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(ChildRootBehaviorKey).Set(WaitedAfterBirthKey, _waitedAfterBirth);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ChildRootBehaviorKey);
		_waitedAfterBirth = component.Get(WaitedAfterBirthKey);
	}
}

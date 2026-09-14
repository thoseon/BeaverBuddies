using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Navigation;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.WalkingSystem;

public class WalkToAccessibleExecutor : BaseComponent, IAwakableComponent, IExecutor
{
	private static readonly ComponentKey WalkToAccessibleExecutorKey = new ComponentKey("WalkToAccessibleExecutor");

	private static readonly PropertyKey<Accessible> AccessibleKey = new PropertyKey<Accessible>("Accessible");

	private static readonly PropertyKey<bool> IgnoreAccessibleValidityKey = new PropertyKey<bool>("IgnoreAccessibleValidity");

	private readonly ReferenceSerializer _referenceSerializer;

	private Walker _walker;

	private Accessible _accessible;

	private bool _ignoreAccessibleValidity;

	public WalkToAccessibleExecutor(ReferenceSerializer referenceSerializer)
	{
		_referenceSerializer = referenceSerializer;
	}

	public void Awake()
	{
		_walker = GetComponent<Walker>();
	}

	public ExecutorStatus LaunchIgnoringAccessibleValidity(Accessible accessible)
	{
		return Launch(accessible, ignoreAccessibleValidity: true);
	}

	public ExecutorStatus Launch(Accessible accessible)
	{
		return Launch(accessible, ignoreAccessibleValidity: false);
	}

	public ExecutorStatus Tick(float deltaTimeInHours)
	{
		if (!_accessible || (!_accessible.ValidAccessible && !_ignoreAccessibleValidity))
		{
			return ExecutorStatus.Failure;
		}
		if (!_walker.CurrentDestinationReachable)
		{
			return ExecutorStatus.Failure;
		}
		if (_walker.Stopped())
		{
			return ExecutorStatus.Success;
		}
		return ExecutorStatus.Running;
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(WalkToAccessibleExecutorKey);
		if ((bool)_accessible)
		{
			component.Set(AccessibleKey, _accessible, _referenceSerializer.Of<Accessible>());
		}
		component.Set(IgnoreAccessibleValidityKey, _ignoreAccessibleValidity);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(WalkToAccessibleExecutorKey);
		if (component.Has(AccessibleKey) && component.GetObsoletable(AccessibleKey, _referenceSerializer.Of<Accessible>(), out var value))
		{
			_accessible = value;
		}
		_ignoreAccessibleValidity = component.Get(IgnoreAccessibleValidityKey);
	}

	private ExecutorStatus Launch(Accessible accessible, bool ignoreAccessibleValidity)
	{
		if (!accessible || (!accessible.ValidAccessible && !ignoreAccessibleValidity))
		{
			return ExecutorStatus.Failure;
		}
		_accessible = accessible;
		_ignoreAccessibleValidity = ignoreAccessibleValidity;
		AccessibleDestination destination = new AccessibleDestination(_accessible);
		ExecutorStatus executorStatus = _walker.GoTo(destination);
		if (executorStatus != ExecutorStatus.Running)
		{
			return executorStatus;
		}
		return ExecutorStatus.Running;
	}
}

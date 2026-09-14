using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Buildings;
using Timberborn.EnterableSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.WalkingSystem;

public class WalkInsideExecutor : BaseComponent, IAwakableComponent, IExecutor
{
	private static readonly ComponentKey WalkInsideExecutorKey = new ComponentKey("WalkInsideExecutor");

	private static readonly PropertyKey<Enterable> EnterableKey = new PropertyKey<Enterable>("Enterable");

	private static readonly PropertyKey<bool> IgnoreAccessibleValidityKey = new PropertyKey<bool>("IgnoreAccessibleValidity");

	private static readonly PropertyKey<bool> LimitWalkTimeKey = new PropertyKey<bool>("LimitWalkTime");

	private static readonly float MaxWalkTimeInHours = 0.15f;

	private readonly ReferenceSerializer _referenceSerializer;

	private Walker _walker;

	private Enterer _enterer;

	private Enterable _enterable;

	private BuildingAccessible _buildingAccessible;

	private bool _ignoreAccessibleValidity;

	private bool _limitWalkTime;

	private float _currentWalkTime;

	public WalkInsideExecutor(ReferenceSerializer referenceSerializer)
	{
		_referenceSerializer = referenceSerializer;
	}

	public void Awake()
	{
		_walker = GetComponent<Walker>();
		_enterer = GetComponent<Enterer>();
	}

	public ExecutorStatus LaunchIgnoringAccessibleValidity(Enterable enterable)
	{
		return Launch(enterable, ignoreAccessibleValidity: true, limitWalkTime: false);
	}

	public ExecutorStatus LaunchForLimitedTime(Enterable enterable)
	{
		return Launch(enterable, ignoreAccessibleValidity: false, limitWalkTime: true);
	}

	public ExecutorStatus Launch(Enterable enterable)
	{
		return Launch(enterable, ignoreAccessibleValidity: false, limitWalkTime: false);
	}

	public ExecutorStatus Tick(float deltaTimeInHours)
	{
		if (!_buildingAccessible || !_buildingAccessible.Accessible || (!_ignoreAccessibleValidity && !_buildingAccessible.Accessible.ValidAccessible) || !_walker.CurrentDestinationReachable)
		{
			_enterer.UnreserveSlot();
			return ExecutorStatus.Failure;
		}
		if (_limitWalkTime)
		{
			_currentWalkTime += deltaTimeInHours;
			if (_currentWalkTime > MaxWalkTimeInHours)
			{
				_walker.StopNextTick();
				_enterer.UnreserveSlot();
				return ExecutorStatus.Success;
			}
		}
		if (_walker.Stopped())
		{
			_enterer.Enter(_enterable);
			return ExecutorStatus.Success;
		}
		return ExecutorStatus.Running;
	}

	public void Save(IEntitySaver entitySaver)
	{
		if ((bool)_enterable)
		{
			IObjectSaver component = entitySaver.GetComponent(WalkInsideExecutorKey);
			component.Set(EnterableKey, _enterable, _referenceSerializer.Of<Enterable>());
			component.Set(IgnoreAccessibleValidityKey, _ignoreAccessibleValidity);
			component.Set(LimitWalkTimeKey, _limitWalkTime);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(WalkInsideExecutorKey, out var objectLoader) && objectLoader.GetObsoletable(EnterableKey, _referenceSerializer.Of<Enterable>(), out var value))
		{
			bool ignoreAccessibleValidity = objectLoader.Get(IgnoreAccessibleValidityKey);
			bool limitWalkTime = objectLoader.Get(LimitWalkTimeKey);
			Initialize(value, ignoreAccessibleValidity, limitWalkTime);
		}
	}

	private ExecutorStatus Launch(Enterable enterable, bool ignoreAccessibleValidity, bool limitWalkTime)
	{
		if (!enterable)
		{
			return ExecutorStatus.Failure;
		}
		Initialize(enterable, ignoreAccessibleValidity, limitWalkTime);
		if (_enterer.CurrentBuilding == enterable)
		{
			return ExecutorStatus.Success;
		}
		_enterer.UnreserveSlot();
		if (!_enterable.CanReserveSlot)
		{
			return ExecutorStatus.Failure;
		}
		AccessibleDestination destination = new AccessibleDestination(_buildingAccessible.Accessible);
		switch (_walker.GoTo(destination))
		{
		case ExecutorStatus.Success:
			_enterer.Enter(_enterable);
			return ExecutorStatus.Success;
		case ExecutorStatus.Failure:
			return ExecutorStatus.Failure;
		default:
			_enterer.ReserveSlot(_enterable);
			return ExecutorStatus.Running;
		}
	}

	private void Initialize(Enterable enterable, bool ignoreAccessibleValidity, bool limitWalkTime)
	{
		_enterable = enterable;
		_ignoreAccessibleValidity = ignoreAccessibleValidity;
		_limitWalkTime = limitWalkTime;
		_buildingAccessible = _enterable.GetComponent<BuildingAccessible>();
		_currentWalkTime = 0f;
	}
}

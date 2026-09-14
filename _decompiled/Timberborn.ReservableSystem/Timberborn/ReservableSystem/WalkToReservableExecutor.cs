using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Persistence;
using Timberborn.WalkingSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.ReservableSystem;

public class WalkToReservableExecutor : BaseComponent, IAwakableComponent, IExecutor
{
	private static readonly ComponentKey WalkToReservableExecutorKey = new ComponentKey("WalkToReservableExecutor");

	private static readonly PropertyKey<ReservableReacher> ReservableReacherKey = new PropertyKey<ReservableReacher>("ReservableReacher");

	private readonly ReferenceSerializer _referenceSerializer;

	private Walker _walker;

	private ReservableReacher _reservableReacher;

	private Reservable _reservable;

	public WalkToReservableExecutor(ReferenceSerializer referenceSerializer)
	{
		_referenceSerializer = referenceSerializer;
	}

	public void Awake()
	{
		_walker = GetComponent<Walker>();
	}

	public ExecutorStatus Launch(ReservableReacher reservableReacher)
	{
		SetTarget(reservableReacher);
		return _walker.GoTo(_reservableReacher.Destination);
	}

	public ExecutorStatus Tick(float deltaTimeInHours)
	{
		if (!_walker.CurrentDestinationReachable || !_reservable || !_reservable.Reserved)
		{
			return ExecutorStatus.Failure;
		}
		if (_walker.Stopped())
		{
			_reservableReacher.NotifyReservableReached((BaseComponent)(object)_walker);
			return ExecutorStatus.Success;
		}
		return ExecutorStatus.Running;
	}

	public void Save(IEntitySaver entitySaver)
	{
		if ((bool)_reservableReacher)
		{
			entitySaver.GetComponent(WalkToReservableExecutorKey).Set(ReservableReacherKey, _reservableReacher, _referenceSerializer.Of<ReservableReacher>());
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(WalkToReservableExecutorKey, out var objectLoader) && objectLoader.GetObsoletable(ReservableReacherKey, _referenceSerializer.Of<ReservableReacher>(), out var value))
		{
			SetTarget(value);
		}
	}

	private void SetTarget(ReservableReacher reservableReacher)
	{
		_reservable = reservableReacher.GetComponent<Reservable>();
		_reservableReacher = reservableReacher;
	}
}

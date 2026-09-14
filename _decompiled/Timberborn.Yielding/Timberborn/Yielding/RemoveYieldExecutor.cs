using Timberborn.ReservableSystem;
using Timberborn.TimeSystem;

namespace Timberborn.Yielding;

public class RemoveYieldExecutor : WorkAtReservableExecutor
{
	private YielderRemover _yielderRemover;

	private YieldRemovalSuccessValidator _yieldRemovalSuccessValidator;

	protected override string Animation => _yielderRemover.ReservedYielder.Animation;

	protected override Reservable Reservable
	{
		get
		{
			if (!_yielderRemover.HasReservedYielder)
			{
				return null;
			}
			return _yielderRemover.ReservedYielder.Reservable;
		}
	}

	protected RemoveYieldExecutor(IDayNightCycle dayNightCycle)
		: base(dayNightCycle)
	{
	}

	public bool Remove()
	{
		if (_yielderRemover.HasReservedYielder)
		{
			Launch(_yielderRemover.ReservedYielder.RemovalTimeInHours);
			return true;
		}
		return false;
	}

	protected override void Initialize()
	{
		_yielderRemover = GetComponent<YielderRemover>();
		_yieldRemovalSuccessValidator = GetComponent<YieldRemovalSuccessValidator>();
	}

	protected override bool CanComplete()
	{
		return _yieldRemovalSuccessValidator.ValidateYieldSuccess();
	}

	protected override void PerformActionOnComplete()
	{
		_yielderRemover.CompleteReservation();
	}

	protected override void PerformActionOnTick(float deltaTime)
	{
	}

	protected override void Unreserve()
	{
		_yielderRemover.Unreserve();
	}
}

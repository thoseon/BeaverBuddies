using Timberborn.BlockSystem;
using Timberborn.ReservableSystem;
using Timberborn.TimeSystem;

namespace Timberborn.Demolishing;

public class DemolishExecutor : WorkAtReservableExecutor
{
	private static readonly float MaxDemolishingTimeInHours = 0.5f;

	private Demolisher _demolisher;

	protected override string Animation => "Building";

	protected override Reservable Reservable
	{
		get
		{
			if (!_demolisher.HasReservedDemolishable)
			{
				return null;
			}
			return _demolisher.Demolishable.Reservable;
		}
	}

	protected DemolishExecutor(IDayNightCycle dayNightCycle)
		: base(dayNightCycle)
	{
	}

	public bool Demolish()
	{
		if (_demolisher.HasReservedDemolishable)
		{
			Launch(MaxDemolishingTimeInHours);
			return true;
		}
		return false;
	}

	protected override void Initialize()
	{
		_demolisher = GetComponent<Demolisher>();
	}

	protected override bool CanComplete()
	{
		if (_demolisher.HasReservedDemolishable && _demolisher.Demolishable.DemolishingProgress >= 1f)
		{
			return _demolisher.Demolishable.GetComponent<BlockObject>().CanDelete();
		}
		return false;
	}

	protected override void PerformActionOnTick(float deltaTime)
	{
		if (_demolisher.HasReservedDemolishable)
		{
			_demolisher.Demolishable.ProgressDemolition(deltaTime);
		}
	}

	protected override void PerformActionOnComplete()
	{
		_demolisher.Demolish();
	}

	protected override void Unreserve()
	{
		_demolisher.Unreserve();
	}
}

using Timberborn.SingletonSystem;

namespace Timberborn.TickSystem;

public class TickOnlyArrayService : ILoadableSingleton, ITickableSingleton
{
	private readonly ITickableSingletonService _tickableSingletonService;

	private bool _isLoadPhase;

	private bool _forceAllowAccess;

	public bool AllowEdit
	{
		get
		{
			if (!_forceAllowAccess && !_tickableSingletonService.ParalleTicklIsFinished)
			{
				return _isLoadPhase;
			}
			return true;
		}
	}

	public bool AllowFullAccess => _tickableSingletonService.IsStartingParallelTick;

	public TickOnlyArrayService(ITickableSingletonService tickableSingletonService)
	{
		_tickableSingletonService = tickableSingletonService;
	}

	public void Load()
	{
		_isLoadPhase = true;
	}

	public void Tick()
	{
		_isLoadPhase = false;
	}

	public TickOnlyArray<T> Create<T>(int size)
	{
		return new TickOnlyArray<T>(size, this);
	}

	public void ForceAllowAccess()
	{
		_forceAllowAccess = true;
	}
}

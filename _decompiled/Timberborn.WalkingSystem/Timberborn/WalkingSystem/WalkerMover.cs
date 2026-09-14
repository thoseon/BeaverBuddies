using Timberborn.BaseComponentSystem;
using Timberborn.EnterableSystem;
using Timberborn.TickSystem;

namespace Timberborn.WalkingSystem;

public class WalkerMover : TickableComponent, IAwakableComponent, ILateTickable
{
	private static readonly string WalkingAnimation = "Walking";

	private readonly ITickService _tickService;

	private Enterer _enterer;

	private Walker _walker;

	private WalkerSpeedManager _walkerSpeedManager;

	public WalkerMover(ITickService tickService)
	{
		_tickService = tickService;
	}

	public void Awake()
	{
		_enterer = GetComponent<Enterer>();
		_walker = GetComponent<Walker>();
		_walkerSpeedManager = GetComponent<WalkerSpeedManager>();
	}

	public override void Tick()
	{
		if (!_walker.Stopped())
		{
			Move();
		}
	}

	private void Move()
	{
		if (_enterer.IsInside)
		{
			_enterer.Exit();
		}
		else
		{
			_walker.PathFollower.MoveAlongPath(_tickService.TickIntervalInSeconds, WalkingAnimation, _walkerSpeedManager.GetWalkerSpeedAtCurrentPosition);
		}
	}
}

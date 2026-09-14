using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WalkingSystem;

public class WalkToPositionExecutor : BaseComponent, IAwakableComponent, IExecutor
{
	private readonly PositionDestinationFactory _positionDestinationFactory;

	private Walker _walker;

	public WalkToPositionExecutor(PositionDestinationFactory positionDestinationFactory)
	{
		_positionDestinationFactory = positionDestinationFactory;
	}

	public void Awake()
	{
		_walker = GetComponent<Walker>();
	}

	public ExecutorStatus Launch(Vector3 position)
	{
		PositionDestination destination = _positionDestinationFactory.Create(position, 0f);
		return _walker.GoTo(destination);
	}

	public ExecutorStatus Tick(float deltaTimeInHours)
	{
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
	}

	public void Load(IEntityLoader entityLoader)
	{
	}
}

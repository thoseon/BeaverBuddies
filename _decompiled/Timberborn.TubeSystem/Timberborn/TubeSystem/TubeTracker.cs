using Timberborn.BaseComponentSystem;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.WalkingSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.TubeSystem;

internal class TubeTracker : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity, IWaterResistor, IWaterPenaltyModifier
{
	private static readonly Vector3 PositionOffset = Vector3.up * 0.1f;

	private readonly TubeMap _tubeMap;

	private readonly EventBus _eventBus;

	private Walker _walker;

	private bool _isInTube;

	public bool IsWaterResistant => _isInTube;

	public float WaterPenaltyModifier => (!_isInTube) ? 1 : 0;

	public TubeTracker(TubeMap tubeMap, EventBus eventBus)
	{
		_tubeMap = tubeMap;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_walker = GetComponent<Walker>();
	}

	public void InitializeEntity()
	{
		CheckTubeInPosition();
		_walker.PathFollower.MovedAlongPath += delegate
		{
			CheckTubeInPosition();
		};
		_eventBus.Register(this);
	}

	public void DeleteEntity()
	{
		_eventBus.Unregister(this);
	}

	[OnEvent]
	public void OnEntityInitialized(EntityInitializedEvent entityInitialized)
	{
		if (!_isInTube && (bool)entityInitialized.Entity.GetComponent<Tube>())
		{
			CheckTubeInPosition();
		}
	}

	[OnEvent]
	public void OnEntityDeleted(EntityDeletedEvent entityDeletedEvent)
	{
		if (_isInTube && (bool)entityDeletedEvent.Entity.GetComponent<Tube>())
		{
			CheckTubeInPosition();
		}
	}

	private void CheckTubeInPosition()
	{
		Vector3Int gridPosition = CoordinateSystem.WorldToGridInt(base.Transform.position + PositionOffset);
		_isInTube = _tubeMap.GetTubeAt(gridPosition)?.CanBeVisited ?? false;
	}
}

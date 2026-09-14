using Timberborn.CharacterModelSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.EnterableSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.SlotSystem;

public class PatrollingSlot : ISlot
{
	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly Transform _slotTransform;

	private readonly Transform _start;

	private readonly Transform _end;

	private readonly PatrollingSlotSpec _patrollingSlotSpec;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private Vector3 _destination;

	private float _movementSpeed;

	public Enterer AssignedEnterer { get; private set; }

	public string Name => _slotTransform.name;

	public bool IsAvailable => _slotTransform.gameObject.activeInHierarchy;

	public PatrollingSlot(IRandomNumberGenerator randomNumberGenerator, Transform slotTransform, Transform start, Transform end, PatrollingSlotSpec patrollingSlotSpec, IThreadSafeWaterMap threadSafeWaterMap)
	{
		_randomNumberGenerator = randomNumberGenerator;
		_slotTransform = slotTransform;
		_start = start;
		_end = end;
		_patrollingSlotSpec = patrollingSlotSpec;
		_threadSafeWaterMap = threadSafeWaterMap;
	}

	public void AssignEnterer(Enterer enterer)
	{
		AssignedEnterer = enterer;
		enterer.GetComponent<CharacterModel>().AnimateFollowingTarget(_slotTransform, _patrollingSlotSpec.Animation);
		_movementSpeed = RandomMovementSpeed();
		RandomizePositionToStartOrEnd();
	}

	public void UnassignEnterer()
	{
		if ((bool)AssignedEnterer)
		{
			AssignedEnterer.GetComponent<CharacterModel>().StopAnimating();
		}
		AssignedEnterer = null;
	}

	public void Update(float deltaTime)
	{
		MoveDestinationToWaterLevel();
		float num = _movementSpeed * deltaTime;
		if (Vector2.Distance(_slotTransform.position.XZ(), _destination.XZ()) < num)
		{
			FlipDirection();
		}
		else
		{
			MoveToDestination(num);
		}
	}

	public override string ToString()
	{
		string text = (AssignedEnterer ? AssignedEnterer.Name : "Nobody");
		return "Slot: PatrollingSlot, assigned: " + text;
	}

	private float RandomMovementSpeed()
	{
		float num = _randomNumberGenerator.Range(1f - _patrollingSlotSpec.MaxRandomDeviationOfMovementSpeed, 1f + _patrollingSlotSpec.MaxRandomDeviationOfMovementSpeed);
		return _patrollingSlotSpec.BaseMovementSpeed * num;
	}

	private void FlipDirection()
	{
		_slotTransform.position = _destination;
		_destination = ((_destination.XZ() == _start.position.XZ()) ? _end.position : _start.position);
	}

	private void MoveToDestination(float distanceToTravel)
	{
		Vector3 vector = (_destination - _slotTransform.position).normalized * distanceToTravel;
		_slotTransform.position += vector;
		_slotTransform.LookAt(_destination);
	}

	private void RandomizePositionToStartOrEnd()
	{
		if (_randomNumberGenerator.Range(0f, 1f) > 0.5f)
		{
			_destination = _start.position;
			_slotTransform.position = _end.position;
		}
		else
		{
			_destination = _end.position;
			_slotTransform.position = _start.position;
		}
		_slotTransform.LookAt(_end);
	}

	private void MoveDestinationToWaterLevel()
	{
		if (_patrollingSlotSpec.WaterSlot)
		{
			Vector3Int coordinates = CoordinateSystem.WorldToGridInt(_destination);
			float y = _threadSafeWaterMap.WaterHeightOrFloor(coordinates);
			_destination.y = y;
		}
	}
}

using Timberborn.CharacterModelSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.EnterableSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.SlotSystem;

public class TransformSlot : ISlot
{
	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private readonly Transform _followedTransform;

	private readonly TransformSlotSpec _transformSlotSpec;

	public Enterer AssignedEnterer { get; private set; }

	public string Name => _followedTransform.name;

	public bool IsAvailable => _followedTransform.gameObject.activeInHierarchy;

	public TransformSlot(IRandomNumberGenerator randomNumberGenerator, IThreadSafeWaterMap threadSafeWaterMap, Transform followedTransform, TransformSlotSpec transformSlotSpec)
	{
		_randomNumberGenerator = randomNumberGenerator;
		_threadSafeWaterMap = threadSafeWaterMap;
		_followedTransform = followedTransform;
		_transformSlotSpec = transformSlotSpec;
	}

	public void AssignEnterer(Enterer enterer)
	{
		AssignedEnterer = enterer;
		CharacterModel component = AssignedEnterer.GetComponent<CharacterModel>();
		if (_transformSlotSpec.RandomizeYRotation)
		{
			_followedTransform.rotation *= Quaternion.AngleAxis(_randomNumberGenerator.Range(0, 360), Vector3.up);
		}
		ImmediatelyMoveSlotToWaterLevel();
		if (_transformSlotSpec.Inanimate)
		{
			component.PositionAtTarget(_followedTransform);
		}
		else
		{
			component.AnimateFollowingTarget(_followedTransform, _transformSlotSpec.Animation);
		}
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
		MoveSlotToWaterLevel(deltaTime);
	}

	public override string ToString()
	{
		string text = (AssignedEnterer ? AssignedEnterer.Name : "Nobody");
		return "Slot: TransformSlot, assigned: " + text;
	}

	private void ImmediatelyMoveSlotToWaterLevel()
	{
		MoveSlotToWaterLevel(10000f);
	}

	private void MoveSlotToWaterLevel(float deltaTime)
	{
		if (_transformSlotSpec.WaterSlot)
		{
			Vector3 position = _followedTransform.position;
			Vector3Int coordinates = CoordinateSystem.WorldToGridInt(position);
			float value = _threadSafeWaterMap.WaterHeightOrFloor(coordinates) - position.y;
			float num = 0.1f * deltaTime;
			position.y += Mathf.Clamp(value, 0f - num, num);
			_followedTransform.position = position;
		}
	}
}

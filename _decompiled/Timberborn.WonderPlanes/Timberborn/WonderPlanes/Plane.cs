using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.Common;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WonderPlanes;

internal class Plane : BaseComponent, IAwakableComponent, IUpdatableComponent, IPersistentEntity
{
	private static readonly ComponentKey PlaneKey = new ComponentKey("Plane");

	private static readonly PropertyKey<float> SpeedKey = new PropertyKey<float>("Speed");

	private static readonly PropertyKey<Vector3> PositionKey = new PropertyKey<Vector3>("Position");

	private static readonly PropertyKey<Quaternion> RotationKey = new PropertyKey<Quaternion>("Rotation");

	private static readonly PropertyKey<bool> IsFreeFlyingKey = new PropertyKey<bool>("IsFreeFlying");

	private PlaneSpec _planeSpec;

	private CharacterModel _pilotCharacterModel;

	private float _speed;

	private bool _isFreeFlying;

	private Quaternion _horizontalRotation;

	public Transform PilotSeatTransform { get; private set; }

	public void Awake()
	{
		_planeSpec = GetComponent<PlaneSpec>();
		PilotSeatTransform = base.GameObject.FindChildTransform(_planeSpec.PilotSeatName);
	}

	public void Update()
	{
		if (_isFreeFlying)
		{
			RotateTowardHorizontalFlight(Time.deltaTime);
		}
		base.Transform.position += base.Transform.forward * (_speed * Time.deltaTime);
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(PlaneKey);
		component.Set(PositionKey, base.Transform.position);
		component.Set(RotationKey, base.Transform.rotation);
		component.Set(SpeedKey, _speed);
		component.Set(IsFreeFlyingKey, _isFreeFlying);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(PlaneKey);
		base.Transform.SetPositionAndRotation(component.Get(PositionKey), component.Get(RotationKey));
		ComputeFinalRotation();
		_speed = component.Get(SpeedKey);
		_isFreeFlying = component.Get(IsFreeFlyingKey);
	}

	public void Initialize(Transform spawnPointTransform)
	{
		base.Transform.SetPositionAndRotation(spawnPointTransform.position, spawnPointTransform.rotation);
		ComputeFinalRotation();
	}

	public void SetSpeed(float speed)
	{
		_speed = speed;
	}

	public void StartFreeFlight()
	{
		_isFreeFlying = true;
	}

	private void ComputeFinalRotation()
	{
		_horizontalRotation = Quaternion.Euler(0f, base.Transform.eulerAngles.y, base.Transform.eulerAngles.z);
	}

	private void RotateTowardHorizontalFlight(float deltaTime)
	{
		base.Transform.rotation = Quaternion.RotateTowards(base.Transform.rotation, _horizontalRotation, deltaTime * _planeSpec.RotationSpeed);
	}
}

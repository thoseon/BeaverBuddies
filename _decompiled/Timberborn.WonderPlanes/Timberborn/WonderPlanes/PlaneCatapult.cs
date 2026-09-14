using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.Localization;
using Timberborn.NotificationSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WonderPlanes;

internal class PlaneCatapult : BaseComponent, IAwakableComponent, IUpdatableComponent, IPersistentEntity
{
	private static readonly float PlaneWaitTimeInSeconds = 1f;

	private static readonly string LaunchedLocKey = "Beaver.Launched";

	private static readonly ComponentKey PlaneCatapultKey = new ComponentKey("PlaneCatapult");

	private static readonly PropertyKey<Plane> CurrentPlaneKey = new PropertyKey<Plane>("CurrentPlane");

	private static readonly float RunwayLength = 10f;

	private readonly NotificationBus _notificationBus;

	private readonly ILoc _loc;

	private readonly ReferenceSerializer _referenceSerializer;

	private PlaneSpawner _planeSpawner;

	private AnimationCurve _speedCurve;

	private PlaneCatapultSpec _planeCatapultSpec;

	private Plane _catapultedPlane;

	private float _remainingPlaneWaitTime;

	public event EventHandler PlaneCatapulted;

	public PlaneCatapult(NotificationBus notificationBus, ILoc loc, ReferenceSerializer referenceSerializer)
	{
		_notificationBus = notificationBus;
		_loc = loc;
		_referenceSerializer = referenceSerializer;
	}

	public void Awake()
	{
		_planeSpawner = GetComponent<PlaneSpawner>();
		_speedCurve = GetComponent<PlaneCatapultSpec>().SpeedCurve.ToAnimationCurve();
		DisableComponent();
	}

	public void Update()
	{
		if ((bool)_catapultedPlane)
		{
			UpdatePlane();
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		if ((bool)_catapultedPlane)
		{
			entitySaver.GetComponent(PlaneCatapultKey).Set(CurrentPlaneKey, _catapultedPlane, _referenceSerializer.Of<Plane>());
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(PlaneCatapultKey, out var objectLoader))
		{
			_catapultedPlane = objectLoader.Get(CurrentPlaneKey, _referenceSerializer.Of<Plane>());
			if ((bool)_catapultedPlane)
			{
				EnableComponent();
			}
		}
	}

	public void CatapultPlane(Pilot pilot)
	{
		if (!_catapultedPlane)
		{
			EnableComponent();
			_remainingPlaneWaitTime = PlaneWaitTimeInSeconds;
			_catapultedPlane = _planeSpawner.SpawnPlane(pilot);
			Character component = pilot.GetComponent<Character>();
			_notificationBus.Post(_loc.T(LaunchedLocKey, component.FirstName), component);
		}
	}

	private void UpdatePlane()
	{
		if (_remainingPlaneWaitTime > 0f)
		{
			_remainingPlaneWaitTime -= Time.deltaTime;
			return;
		}
		float num = Vector3.Magnitude(_catapultedPlane.Transform.position - _planeSpawner.SpawnPosition) / RunwayLength;
		_catapultedPlane.SetSpeed(_speedCurve.Evaluate(num));
		if (num >= 1f)
		{
			_catapultedPlane.StartFreeFlight();
			_catapultedPlane = null;
			DisableComponent();
			PlaneCatapulted?.Invoke(this, EventArgs.Empty);
		}
	}
}

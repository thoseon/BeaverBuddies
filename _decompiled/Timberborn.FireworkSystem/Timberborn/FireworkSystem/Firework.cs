using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Particles;
using Timberborn.Persistence;
using Timberborn.SoundSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.FireworkSystem;

internal class Firework : BaseComponent, IAwakableComponent, IPostInitializableEntity, IUpdatableComponent, IPersistentEntity
{
	private static readonly float Thrust = 10f;

	private static readonly float InitialVelocity = 10f;

	private static readonly float DistanceMargin = 10f;

	private static readonly float LoadedParticlesFastForward = 0.1f;

	private static readonly Vector3 Gravity = new Vector3(0f, -9.81f, 0f);

	private static readonly ComponentKey ComponentKey = new ComponentKey("Firework");

	private static readonly PropertyKey<Vector3> PositionKey = new PropertyKey<Vector3>("Position");

	private static readonly PropertyKey<Quaternion> RotationKey = new PropertyKey<Quaternion>("Rotation");

	private static readonly PropertyKey<Vector3> InitialPositionKey = new PropertyKey<Vector3>("InitialPosition");

	private static readonly PropertyKey<Vector3> InitialDirectionKey = new PropertyKey<Vector3>("InitialDirection");

	private static readonly PropertyKey<float> FightDistanceKey = new PropertyKey<float>("FlightDistanceKey");

	private static readonly PropertyKey<float> DistanceFlownKey = new PropertyKey<float>("DistanceFlown");

	private static readonly PropertyKey<float> SimulationTimeKey = new PropertyKey<float>("SimulationTime");

	private static readonly PropertyKey<float> SpawnTimestampKey = new PropertyKey<float>("SpawnTimestamp");

	private readonly EntityService _entityService;

	private readonly ISoundSystem _soundSystem;

	private readonly IDayNightCycle _dayNightCycle;

	private FireworkSpec _spec;

	private ParticlesRunner _trailRunner;

	private ParticlesRunner _burstRunner;

	private bool _isFinalized;

	private bool _isDeleted;

	private Vector3 _initialPosition;

	private Vector3 _initialDirection;

	private float _flightDistance;

	private float _distanceFlown;

	private float _simulationTime;

	private float _spawnTimestamp;

	private bool _isLoaded;

	public Firework(EntityService entityService, ISoundSystem soundSystem, IDayNightCycle dayNightCycle)
	{
		_entityService = entityService;
		_soundSystem = soundSystem;
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_spec = GetComponent<FireworkSpec>();
	}

	public void PostInitializeEntity()
	{
		InitializeParticles();
		if (!_isLoaded)
		{
			PlayTrailSound();
		}
	}

	public void Launch(Vector3 position, Quaternion quaternion, float flightDistance)
	{
		_spawnTimestamp = _dayNightCycle.FluidSecondsPassedToday;
		_initialPosition = position;
		_initialDirection = (quaternion * Vector3.forward).normalized;
		_flightDistance = flightDistance;
		base.Transform.position = position;
		base.Transform.rotation = quaternion;
	}

	public void Update()
	{
		if (!_isDeleted && Time.timeScale > 0f)
		{
			UpdateFinalization();
			UpdateMovement();
			UpdateDeletion();
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (!_isFinalized)
		{
			IObjectSaver component = entitySaver.GetComponent(ComponentKey);
			component.Set(PositionKey, base.Transform.position);
			component.Set(RotationKey, base.Transform.rotation);
			component.Set(InitialPositionKey, _initialPosition);
			component.Set(InitialDirectionKey, _initialDirection);
			component.Set(FightDistanceKey, _flightDistance);
			component.Set(DistanceFlownKey, _distanceFlown);
			component.Set(SimulationTimeKey, _simulationTime);
			component.Set(SpawnTimestampKey, _spawnTimestamp);
		}
	}

	[BackwardCompatible(2026, 3, 5, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(ComponentKey, out var objectLoader) && objectLoader.Has(InitialPositionKey))
		{
			base.Transform.position = objectLoader.Get(PositionKey);
			base.Transform.rotation = objectLoader.Get(RotationKey);
			_initialPosition = objectLoader.Get(InitialPositionKey);
			_initialDirection = objectLoader.Get(InitialDirectionKey);
			_flightDistance = objectLoader.Get(FightDistanceKey);
			_distanceFlown = objectLoader.Get(DistanceFlownKey);
			_simulationTime = objectLoader.Get(SimulationTimeKey);
			_spawnTimestamp = objectLoader.Get(SpawnTimestampKey);
			_isLoaded = true;
		}
	}

	private void InitializeParticles()
	{
		ParticlesCache component = GetComponent<ParticlesCache>();
		if (_spec.HasBurst)
		{
			_burstRunner = component.GetParticlesRunner("Burst");
			_burstRunner.Disable();
		}
		_trailRunner = component.GetParticlesRunner("Trail");
		_trailRunner.Enable();
		if (_isLoaded)
		{
			_trailRunner.FastForward(LoadedParticlesFastForward);
		}
		_trailRunner.Play();
	}

	private void UpdateFinalization()
	{
		if (!_isFinalized && _distanceFlown >= _flightDistance)
		{
			_trailRunner.DisableEmission();
			if (_spec.HasBurst)
			{
				PlayBurstSound();
				_burstRunner.Enable();
				_burstRunner.Play();
			}
			_isFinalized = true;
		}
	}

	private void UpdateMovement()
	{
		if (_distanceFlown <= _flightDistance + DistanceMargin)
		{
			_simulationTime = _dayNightCycle.FluidSecondsPassedToday - _spawnTimestamp;
			Vector3 vector = Gravity + Thrust * _initialDirection;
			Vector3 vector2 = _initialPosition + _initialDirection * InitialVelocity * _simulationTime + 0.5f * vector * _simulationTime * _simulationTime;
			_distanceFlown += Vector3.Distance(base.Transform.position, vector2);
			base.Transform.position = vector2;
		}
	}

	private void UpdateDeletion()
	{
		if (_distanceFlown >= DistanceMargin)
		{
			ParticlesRunner burstRunner = _burstRunner;
			if ((burstRunner == null || !burstRunner.HasParticles()) && !_trailRunner.HasParticles())
			{
				_isDeleted = true;
				_entityService.Delete(this);
			}
		}
	}

	private void PlayTrailSound()
	{
		if (!string.IsNullOrWhiteSpace(_spec.TrailSound))
		{
			_soundSystem.PlaySound3D(base.GameObject, _spec.TrailSound, 5);
		}
	}

	private void PlayBurstSound()
	{
		if (!string.IsNullOrWhiteSpace(_spec.BurstSound))
		{
			_soundSystem.PlaySound2D(base.GameObject, _spec.BurstSound, 5);
		}
	}
}

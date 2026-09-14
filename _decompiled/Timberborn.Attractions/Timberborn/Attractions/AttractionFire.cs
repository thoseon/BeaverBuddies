using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using UnityEngine;

namespace Timberborn.Attractions;

public class AttractionFire : TickableComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly EventBus _eventBus;

	private readonly IDayNightCycle _dayNightCycle;

	private GameObject _woodstack;

	private BlockableObject _blockableObject;

	private Fire _fire;

	private float _initialLightIntensity;

	private float _initialFlamesStartSizeConstMax;

	private float _initialFlamesStartLifetimeConstMax;

	private Vector3 _initialWoodstackScale;

	private bool _fireIsOn;

	public AttractionFire(EventBus eventBus, IDayNightCycle dayNightCycle)
	{
		_eventBus = eventBus;
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		string woodstackName = GetComponent<AttractionFireSpec>().WoodstackName;
		if (!string.IsNullOrWhiteSpace(woodstackName))
		{
			_woodstack = base.GameObject.FindChild(woodstackName);
			_initialWoodstackScale = _woodstack.transform.localScale;
		}
		_blockableObject = GetComponent<BlockableObject>();
		_fire = GetComponent<Fire>();
		DisableComponent();
	}

	public override void Tick()
	{
		if (_fireIsOn)
		{
			UpdateFireAnimation();
		}
	}

	public void OnEnterFinishedState()
	{
		_eventBus.Register(this);
		_blockableObject.ObjectBlocked += OnObjectBlocked;
		_blockableObject.ObjectUnblocked += OnObjectUnblocked;
		EnableComponent();
		ParticleSystem.MainModule singleFlame = _fire.SingleFlame;
		_initialFlamesStartSizeConstMax = singleFlame.startSize.constantMax;
		_initialFlamesStartLifetimeConstMax = singleFlame.startLifetime.constantMax;
		_initialLightIntensity = _fire.Light.intensity;
		UpdateFireState();
	}

	public void OnExitFinishedState()
	{
		_eventBus.Unregister(this);
		_blockableObject.ObjectBlocked -= OnObjectBlocked;
		_blockableObject.ObjectUnblocked -= OnObjectUnblocked;
		DisableComponent();
	}

	[OnEvent]
	public void OnNighttimeStartEvent(NighttimeStartEvent nighttimeStartEvent)
	{
		UpdateFireState();
	}

	[OnEvent]
	public void OnDaytimeStartEvent(DaytimeStartEvent daytimeStartEvent)
	{
		UpdateFireState();
	}

	private void OnObjectUnblocked(object sender, EventArgs e)
	{
		UpdateFireState();
	}

	private void OnObjectBlocked(object sender, EventArgs e)
	{
		UpdateFireState();
	}

	private void UpdateFireState()
	{
		bool flag = _blockableObject.IsUnblocked && _dayNightCycle.IsNighttime;
		if (!_fireIsOn & flag)
		{
			StartFire();
		}
		else if (_fireIsOn && !flag)
		{
			StopFire();
		}
	}

	private void StartFire()
	{
		_fire.Enable();
		if ((bool)_woodstack)
		{
			_woodstack.SetActive(value: true);
		}
		_fireIsOn = true;
		UpdateFireAnimation();
	}

	private void StopFire()
	{
		_fire.Disable();
		if ((bool)_woodstack)
		{
			_woodstack.SetActive(value: false);
		}
		_fireIsOn = false;
	}

	private void UpdateFireAnimation()
	{
		float num = _dayNightCycle.HoursToNextStartOf(TimeOfDay.Daytime) / _dayNightCycle.NighttimeLengthInHours;
		float num2 = Mathf.Min(num + 0.25f, 1f);
		ParticleSystem.MainModule singleFlame = _fire.SingleFlame;
		singleFlame.startSize = new ParticleSystem.MinMaxCurve(singleFlame.startSize.constantMin, num2 * _initialFlamesStartSizeConstMax);
		singleFlame.startLifetime = new ParticleSystem.MinMaxCurve(singleFlame.startLifetime.constantMin, num2 * _initialFlamesStartLifetimeConstMax);
		_fire.SetSpeedMultiplier(num2);
		_fire.Light.intensity = _initialLightIntensity * num2;
		if ((bool)_woodstack)
		{
			_woodstack.transform.localScale = new Vector3(_initialWoodstackScale.x, num, _initialWoodstackScale.z);
		}
	}
}

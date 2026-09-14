using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.Buildings;

public class FireIntensityController : BaseComponent, IUpdatableComponent, IInitializableEntity
{
	private float _initialStartSizeMultiplier;

	private float _initialStartLifetimeMultiplier;

	private float _modificationEndTimestamp;

	private bool _modificationEnabled;

	private ParticleSystem.MainModule _flame;

	public void InitializeEntity()
	{
		_flame = GetComponent<Fire>().SingleFlame;
		_initialStartSizeMultiplier = _flame.startSizeMultiplier;
		_initialStartLifetimeMultiplier = _flame.startLifetimeMultiplier;
	}

	public void Update()
	{
		if (_modificationEnabled && _modificationEndTimestamp < Time.time)
		{
			SetIntensity(1f, 1f);
			_modificationEnabled = false;
		}
	}

	public void Strengthen()
	{
		StartFlameModification(2.5f, 3.1f, 0.25f);
	}

	public void Dampen()
	{
		StartFlameModification(0f, 0f, 0.5f);
	}

	private void StartFlameModification(float sizeMultiplier, float lifetimeMultiplier, float duration)
	{
		SetIntensity(sizeMultiplier, lifetimeMultiplier);
		_modificationEndTimestamp = Time.time + duration;
		_modificationEnabled = true;
	}

	private void SetIntensity(float sizeMultiplier, float lifetimeMultiplier)
	{
		_flame.startSizeMultiplier = _initialStartSizeMultiplier * sizeMultiplier;
		_flame.startLifetimeMultiplier = _initialStartLifetimeMultiplier * lifetimeMultiplier;
	}
}

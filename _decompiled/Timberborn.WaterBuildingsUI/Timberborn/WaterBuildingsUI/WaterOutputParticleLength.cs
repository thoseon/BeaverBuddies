using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.TickSystem;
using Timberborn.WaterBuildings;
using UnityEngine;

namespace Timberborn.WaterBuildingsUI;

internal class WaterOutputParticleLength : TickableComponent, IAwakableComponent, IInitializableEntity, IPostLoadableEntity, IFinishedStateListener
{
	private WaterOutputParticleSpec _spec;

	private WaterOutput _waterOutput;

	private ParticleSystem.MainModule _particlesMainModule;

	public void Awake()
	{
		_spec = GetComponent<WaterOutputParticleSpec>();
		_waterOutput = GetComponent<WaterOutput>();
		DisableComponent();
	}

	public void InitializeEntity()
	{
		_particlesMainModule = GetComponent<WaterOutputParticle>().ParticleSystem.main;
	}

	public void PostLoadEntity()
	{
		UpdateLifetime();
	}

	public override void Tick()
	{
		UpdateLifetime();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	private void UpdateLifetime()
	{
		float num = _waterOutput.DistanceToGround + _spec.SpawnOffset;
		_particlesMainModule.startLifetime = Math.Max(_spec.MinimumLifetime, num * (1f / _particlesMainModule.startSpeedMultiplier));
	}
}

using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.WaterBuildings;
using UnityEngine;

namespace Timberborn.WaterBuildingsUI;

internal class WaterOutputParticleColorer : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private readonly WaterOutputParticleColors _waterOutputParticleColors;

	private WaterOutputParticleSpec _waterOutputParticleSpec;

	private WaterOutput _waterOutput;

	private ParticleSystem.MainModule _particlesMainModule;

	public WaterOutputParticleColorer(WaterOutputParticleColors waterOutputParticleColors)
	{
		_waterOutputParticleColors = waterOutputParticleColors;
	}

	public void Awake()
	{
		_waterOutputParticleSpec = GetComponent<WaterOutputParticleSpec>();
		_waterOutput = GetComponent<WaterOutput>();
	}

	public void InitializeEntity()
	{
		_particlesMainModule = GetComponent<WaterOutputParticle>().ParticleSystem.main;
		GetComponent<WaterOutput>().WaterAdded += OnWaterAdded;
	}

	private void OnWaterAdded(object sender, WaterAddition e)
	{
		ParticleSystem.MinMaxGradient startColor = _particlesMainModule.startColor;
		float time = e.ContaminatedWater / (e.CleanWater + e.ContaminatedWater);
		Color color = _waterOutputParticleColors.WaterContaminationParticleGradient.Evaluate(time);
		float num = Mathf.Max(0f, _waterOutput.DistanceToGround);
		color.a *= 1f - Mathf.Clamp01(_waterOutputParticleSpec.FadeDistance / num);
		startColor.color = color;
		_particlesMainModule.startColor = startColor;
	}
}

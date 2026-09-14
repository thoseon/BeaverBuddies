using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Forestry;
using Timberborn.TimeSystem;
using Timberborn.TransformControl;
using UnityEngine;

namespace Timberborn.ForestryEffects;

internal class TreeShaker : BaseComponent, IAwakableComponent, IInitializableEntity, IUpdatableComponent
{
	private static readonly int IsShakingPropertyId = Shader.PropertyToID("_IsShaking");

	private static readonly float DoublePi = MathF.PI * 2f;

	private static readonly float SpeedMultiplier = 2.5f * DoublePi;

	private static readonly float AmplitudeDivider = 3f;

	private static readonly float CycleTime = 2f;

	private static readonly float PerlinNoiseMultiplier = 0.35f;

	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private RotationModifier _rotationModifier;

	private float _timer;

	private Vector3 _axis;

	private readonly List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();

	public TreeShaker(NonlinearAnimationManager nonlinearAnimationManager, IRandomNumberGenerator randomNumberGenerator)
	{
		_nonlinearAnimationManager = nonlinearAnimationManager;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Awake()
	{
		_rotationModifier = GetComponent<TransformController>().AddRotationModifier(20);
		TreeRemoveYieldStrategy component = GetComponent<TreeRemoveYieldStrategy>();
		component.CuttingStarted += StartShaking;
		component.CuttingStopped += FinishShaking;
		DisableComponent();
	}

	public void InitializeEntity()
	{
		base.GameObject.GetComponentsInChildren(includeInactive: true, _meshRenderers);
	}

	public void Update()
	{
		UpdateTimer();
		_rotationModifier.Set(Quaternion.AngleAxis(GetCurrentAngle(), _axis));
	}

	private void StartShaking(object sender, TreeCutter treeCutter)
	{
		EnableComponent();
		_timer = _randomNumberGenerator.Range(0f, CycleTime);
		_axis = treeCutter.GetComponent<CharacterModel>().Model.forward;
		UpdateShakingInMaterials(isShaking: true);
	}

	private void FinishShaking(object sender, EventArgs eventArgs)
	{
		DisableComponent();
		_timer = 0f;
		_rotationModifier.Reset();
		UpdateShakingInMaterials(isShaking: false);
	}

	private void UpdateTimer()
	{
		if (_timer > CycleTime)
		{
			_timer = 0f;
		}
		_timer += Time.deltaTime * _nonlinearAnimationManager.SpeedMultiplier;
	}

	private float GetCurrentAngle()
	{
		return Mathf.Sin(_timer * SpeedMultiplier + GetSpeedNoise()) * GetCyclicAmplitudeDistortion() / (AmplitudeDivider + GetAmplitudeNoise());
	}

	private float GetSpeedNoise()
	{
		return Mathf.PerlinNoise1D(_timer * 2f) * PerlinNoiseMultiplier;
	}

	private float GetCyclicAmplitudeDistortion()
	{
		float num = 0.5f * CycleTime;
		return Mathf.Pow(Mathf.Abs(_timer - num) / num, 2f) + 0.5f;
	}

	private float GetAmplitudeNoise()
	{
		return (Mathf.PerlinNoise1D(_timer) - 0.5f) * PerlinNoiseMultiplier;
	}

	private void UpdateShakingInMaterials(bool isShaking)
	{
		float value = (isShaking ? 1f : 0f);
		foreach (MeshRenderer meshRenderer in _meshRenderers)
		{
			meshRenderer.material.SetFloat(IsShakingPropertyId, value);
		}
	}
}

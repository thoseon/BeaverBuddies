using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.TimeSystem;
using Timberborn.Workshops;
using UnityEngine;

namespace Timberborn.WorkshopsEffects;

internal class ObservatoryAnimator : BaseComponent, IAwakableComponent, IUpdatableComponent, IFinishedStateListener
{
	private static readonly float DomeRotationSpeed = 40f;

	private static readonly float TelescopeRotationSpeed = 25f;

	private static readonly float MinDomeRotationAngle = 0f;

	private static readonly float MaxDomeRotationAngle = 360f;

	private static readonly float MinTelescopeRotationAngle = 5f;

	private static readonly float MaxTelescopeRotationAngle = 70f;

	private static readonly float MinGenerationInterval = 1f;

	private static readonly float MaxGenerationInterval = 3f;

	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly IDayNightCycle _dayNightCycle;

	private Workshop _workshop;

	private Transform _dome;

	private Transform _telescope;

	private Quaternion _targetDomeRotation;

	private Quaternion _targetTelescopeRotation;

	private float _nextGenerationTime;

	public ObservatoryAnimator(NonlinearAnimationManager nonlinearAnimationManager, IRandomNumberGenerator randomNumberGenerator, IDayNightCycle dayNightCycle)
	{
		_nonlinearAnimationManager = nonlinearAnimationManager;
		_randomNumberGenerator = randomNumberGenerator;
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		ObservatoryAnimatorSpec component = GetComponent<ObservatoryAnimatorSpec>();
		_dome = base.GameObject.FindChildTransform(component.DomeName);
		_telescope = base.GameObject.FindChildTransform(component.TelescopeName);
		_workshop = GetComponent<Workshop>();
		DisableComponent();
		GenerateRandomAngles();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public void Update()
	{
		if (_dayNightCycle.PartialDayNumber >= _nextGenerationTime)
		{
			GenerateRandomAngles();
		}
		UpdateAnimation();
	}

	private void GenerateRandomAngles()
	{
		float y = _randomNumberGenerator.Range(MinDomeRotationAngle, MaxDomeRotationAngle);
		float num = _randomNumberGenerator.Range(MinTelescopeRotationAngle, MaxTelescopeRotationAngle);
		_targetDomeRotation = Quaternion.Euler(0f, y, 0f);
		_targetTelescopeRotation = Quaternion.Euler(0f - num, 0f, 0f);
		_nextGenerationTime = _dayNightCycle.DayNumberHoursFromNow(_randomNumberGenerator.Range(MinGenerationInterval, MaxGenerationInterval));
	}

	private void UpdateAnimation()
	{
		if (_workshop.CurrentlyWorking)
		{
			float deltaTime = Time.deltaTime * _nonlinearAnimationManager.SpeedMultiplier;
			RotateDome(deltaTime);
			RotateTelescope(deltaTime);
		}
	}

	private void RotateDome(float deltaTime)
	{
		_dome.localRotation = Quaternion.RotateTowards(_dome.localRotation, _targetDomeRotation, deltaTime * DomeRotationSpeed);
	}

	private void RotateTelescope(float deltaTime)
	{
		_telescope.localRotation = Quaternion.RotateTowards(_telescope.localRotation, _targetTelescopeRotation, deltaTime * TelescopeRotationSpeed);
	}
}

using Timberborn.ActivatorSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Illumination;
using Timberborn.TimeSystem;
using UnityEngine;

namespace Timberborn.Explosions;

internal class UnstableCoreLighting : BaseComponent, IAwakableComponent, IInitializableEntity, IUpdatableComponent
{
	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	private TimedComponentActivator _timedComponentActivator;

	private UnstableCoreLightingSpec _spec;

	private float _lastStateChange;

	private IlluminatorToggle _illuminatorToggle;

	public UnstableCoreLighting(NonlinearAnimationManager nonlinearAnimationManager)
	{
		_nonlinearAnimationManager = nonlinearAnimationManager;
	}

	public void Awake()
	{
		_timedComponentActivator = GetComponent<TimedComponentActivator>();
		_spec = GetComponent<UnstableCoreLightingSpec>();
	}

	public void InitializeEntity()
	{
		_illuminatorToggle = GetComponent<Illuminator>().CreateToggle();
		if (_timedComponentActivator.CountdownIsActive)
		{
			_lastStateChange = Time.time;
			return;
		}
		_timedComponentActivator.CountdownActivated += delegate
		{
			_lastStateChange = Time.time;
		};
	}

	public void Update()
	{
		if (_timedComponentActivator.CountdownIsActive && Time.timeScale > 0f)
		{
			float time = Time.time;
			float num = Mathf.Lerp(_spec.MaxInterval, _spec.MinInterval, _timedComponentActivator.ActivationProgress) / _nonlinearAnimationManager.SpeedMultiplier;
			if (time >= _lastStateChange + num)
			{
				_illuminatorToggle.Toggle();
				_lastStateChange = time;
			}
		}
	}
}

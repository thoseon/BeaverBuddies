using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;

namespace Timberborn.Particles;

public class NonLinearParticlesSpeedMultiplier : BaseComponent, IDeletableEntity, IParticlesSpeedMultiplier
{
	private readonly EventBus _eventBus;

	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	private readonly List<ParticlesRunner> _particlesRunner = new List<ParticlesRunner>();

	private bool _registered;

	public float SpeedMultiplier => _nonlinearAnimationManager.SpeedMultiplier;

	public NonLinearParticlesSpeedMultiplier(EventBus eventBus, NonlinearAnimationManager nonlinearAnimationManager)
	{
		_eventBus = eventBus;
		_nonlinearAnimationManager = nonlinearAnimationManager;
	}

	public void AddParticlesRunner(ParticlesRunner particlesRunner)
	{
		if (base.Enabled)
		{
			_particlesRunner.Add(particlesRunner);
			particlesRunner.AddParticleSpeedMultiplier(this);
			Register();
		}
	}

	public void DeleteEntity()
	{
		Unregister();
	}

	public void Disable()
	{
		if (!base.Enabled)
		{
			return;
		}
		DisableComponent();
		foreach (ParticlesRunner item in _particlesRunner)
		{
			item.RemoveParticleSpeedMultiplier(this);
		}
		_particlesRunner.Clear();
		Unregister();
	}

	[OnEvent]
	public void OnCurrentSpeedChanged(CurrentSpeedChangedEvent currentSpeedChangedEvent)
	{
		UpdateSimulationSpeed();
	}

	private void Register()
	{
		if (!_registered)
		{
			_registered = true;
			_eventBus.Register(this);
		}
	}

	private void Unregister()
	{
		if (_registered)
		{
			_registered = false;
			_eventBus.Unregister(this);
		}
	}

	private void UpdateSimulationSpeed()
	{
		foreach (ParticlesRunner item in _particlesRunner)
		{
			item.UpdateSimulationSpeed();
		}
	}
}

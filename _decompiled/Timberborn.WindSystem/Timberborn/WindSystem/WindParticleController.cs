using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Particles;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.WindSystem;

internal class WindParticleController : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener
{
	private readonly EventBus _eventBus;

	private readonly WindService _windService;

	private ParticlesCache _particlesCache;

	private ParticlesRunner _particlesRunner;

	public WindParticleController(EventBus eventBus, WindService windService)
	{
		_eventBus = eventBus;
		_windService = windService;
	}

	public void Awake()
	{
		_particlesCache = GetComponent<ParticlesCache>();
	}

	public void InitializeEntity()
	{
		WindParticleControllerSpec component = GetComponent<WindParticleControllerSpec>();
		_particlesRunner = _particlesCache.GetParticlesRunner(component.AttachmentIds);
	}

	public void OnEnterFinishedState()
	{
		_eventBus.Register(this);
		UpdateParticles();
	}

	public void OnExitFinishedState()
	{
		_eventBus.Unregister(this);
	}

	[OnEvent]
	public void OnWindChanged(WindChangedEvent windChangedEvent)
	{
		UpdateParticles();
	}

	private void UpdateParticles()
	{
		Vector3 forceMultiplier = new Vector3(_windService.WindDirection.x * _windService.WindStrength, 1f, (0f - _windService.WindDirection.y) * _windService.WindStrength);
		_particlesRunner.UpdateForceMultiplier(forceMultiplier);
	}
}

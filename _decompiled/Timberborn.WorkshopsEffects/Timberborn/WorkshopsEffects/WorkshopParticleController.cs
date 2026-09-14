using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Particles;
using Timberborn.Workshops;

namespace Timberborn.WorkshopsEffects;

internal class WorkshopParticleController : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener
{
	private Workshop _workshop;

	private ParticlesRunner _particlesRunner;

	public void Awake()
	{
		_workshop = GetComponent<Workshop>();
	}

	public void InitializeEntity()
	{
		ImmutableArray<string> attachmentIds = GetComponent<WorkshopParticleControllerSpec>().AttachmentIds;
		_particlesRunner = GetComponent<ParticlesCache>().GetParticlesRunner(attachmentIds);
	}

	public void OnEnterFinishedState()
	{
		_workshop.WorkshopStateChanged += OnWorkshopStateChanged;
	}

	public void OnExitFinishedState()
	{
		_workshop.WorkshopStateChanged -= OnWorkshopStateChanged;
	}

	private void OnWorkshopStateChanged(object sender, WorkshopStateChangedEventArgs e)
	{
		if (_workshop.CurrentlyWorking)
		{
			_particlesRunner.Play();
		}
		else
		{
			_particlesRunner.Stop();
		}
	}
}

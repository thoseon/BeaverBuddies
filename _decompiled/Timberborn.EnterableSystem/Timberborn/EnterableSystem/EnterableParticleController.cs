using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Particles;

namespace Timberborn.EnterableSystem;

internal class EnterableParticleController : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener
{
	private Enterable _enterable;

	private ParticlesRunner _particlesRunner;

	public void Awake()
	{
		_enterable = GetComponent<Enterable>();
	}

	public void InitializeEntity()
	{
		ImmutableArray<string> attachmentIds = GetComponent<EnterableParticleControllerSpec>().AttachmentIds;
		_particlesRunner = GetComponent<ParticlesCache>().GetParticlesRunner(attachmentIds);
	}

	public void OnEnterFinishedState()
	{
		_enterable.EntererAdded += OnEntererAdded;
		_enterable.EntererRemoved += OnEntererRemoved;
		UpdateParticles();
	}

	public void OnExitFinishedState()
	{
		_enterable.EntererAdded -= OnEntererAdded;
		_enterable.EntererRemoved -= OnEntererRemoved;
	}

	private void OnEntererAdded(object sender, EntererAddedEventArgs e)
	{
		UpdateParticles();
	}

	private void OnEntererRemoved(object sender, EntererRemovedEventArgs e)
	{
		UpdateParticles();
	}

	private void UpdateParticles()
	{
		if (_enterable.NumberOfEnterersInside > 0)
		{
			_particlesRunner.Play();
		}
		else
		{
			_particlesRunner.Stop();
		}
	}
}

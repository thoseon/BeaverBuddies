using System;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Particles;

namespace Timberborn.BlockingSystem;

internal class BlockableObjectParticleController : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener
{
	private BlockableObject _blockableObject;

	private ParticlesRunner _particlesRunner;

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
	}

	public void InitializeEntity()
	{
		ImmutableArray<string> attachmentIds = GetComponent<BlockableObjectParticleControllerSpec>().AttachmentIds;
		_particlesRunner = GetComponent<ParticlesCache>().GetParticlesRunner(attachmentIds);
	}

	public void OnEnterFinishedState()
	{
		_blockableObject.ObjectUnblocked += OnObjectUnblocked;
		_blockableObject.ObjectBlocked += OnObjectBlocked;
		if (_blockableObject.IsUnblocked)
		{
			_particlesRunner.Play();
		}
	}

	public void OnExitFinishedState()
	{
		_blockableObject.ObjectUnblocked -= OnObjectUnblocked;
		_blockableObject.ObjectBlocked -= OnObjectBlocked;
	}

	private void OnObjectUnblocked(object sender, EventArgs e)
	{
		_particlesRunner.Play();
	}

	private void OnObjectBlocked(object sender, EventArgs e)
	{
		_particlesRunner.Stop();
	}
}

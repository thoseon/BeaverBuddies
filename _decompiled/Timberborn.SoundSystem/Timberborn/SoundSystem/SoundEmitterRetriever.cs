using Bindito.Unity;
using UnityEngine;

namespace Timberborn.SoundSystem;

internal class SoundEmitterRetriever
{
	private readonly IInstantiator _instantiator;

	public SoundEmitterRetriever(IInstantiator instantiator)
	{
		_instantiator = instantiator;
	}

	public SoundEmitter GetSoundEmitter(GameObject emitter)
	{
		if (emitter.TryGetComponent<SoundEmitter>(out var component))
		{
			return component;
		}
		_instantiator.AddComponent<Sounds>(emitter);
		_instantiator.AddComponent<LoopingSoundPlayer>(emitter);
		return _instantiator.AddComponent<SoundEmitter>(emitter);
	}
}

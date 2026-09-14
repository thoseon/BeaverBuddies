using Timberborn.SoundSystem;
using UnityEngine;

namespace Timberborn.Explosions;

public class ExplosionSoundPlayer
{
	private readonly ISoundSystem _soundSystem;

	public ExplosionSoundPlayer(ISoundSystem soundSystem)
	{
		_soundSystem = soundSystem;
	}

	public void Play(GameObject emitter)
	{
		_soundSystem.PlaySound3D(emitter, "Environment.Buildings.DynamiteExplosion", 30);
	}

	public void PlayGlobal(GameObject emitter)
	{
		_soundSystem.PlaySound2D(emitter, "Environment.Buildings.DynamiteExplosion", 30);
	}
}

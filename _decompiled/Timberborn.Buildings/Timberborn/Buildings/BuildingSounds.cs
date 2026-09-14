using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.CoreSound;
using Timberborn.SoundSystem;
using UnityEngine;

namespace Timberborn.Buildings;

public class BuildingSounds : BaseComponent, IAwakableComponent
{
	private readonly ISoundSystem _soundSystem;

	private Building _building;

	private bool _isStarted;

	private bool HasLoopingSound => !string.IsNullOrEmpty(_building.Spec.LoopingSoundName);

	public BuildingSounds(ISoundSystem soundSystem)
	{
		_soundSystem = soundSystem;
	}

	public void Awake()
	{
		_building = GetComponent<Building>();
	}

	public void ToggleSound(bool start)
	{
		if (HasLoopingSound && _isStarted != start)
		{
			_isStarted = start;
			if (_isStarted)
			{
				StartSound(_building);
			}
			else
			{
				StopSound(_building);
			}
		}
	}

	private void StartSound(Building emitter)
	{
		string soundName = GetSoundName(emitter);
		_soundSystem.LoopSingle3DSound(emitter.GameObject, GetSoundName(emitter), 128, GetSoundOffset(emitter));
		_soundSystem.SetCustomMixer(emitter.GameObject, soundName, MixerNames.BuildingMixerNameKey);
	}

	private void StopSound(Building emitter)
	{
		_soundSystem.StopSound(emitter.GameObject, GetSoundName(emitter));
	}

	private static string GetSoundName(Building emitter)
	{
		return "Environment.Buildings.Loop." + emitter.Spec.LoopingSoundName;
	}

	private static Vector3 GetSoundOffset(Building emitter)
	{
		BlockObjectCenter component = emitter.GetComponent<BlockObjectCenter>();
		Vector3 vector = emitter.Transform.position - component.WorldCenter;
		return new Vector3(Mathf.Abs(vector.x), 0f, Mathf.Abs(vector.z));
	}
}

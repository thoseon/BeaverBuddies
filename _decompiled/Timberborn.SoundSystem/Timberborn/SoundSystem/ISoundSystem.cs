using System;
using UnityEngine;

namespace Timberborn.SoundSystem;

public interface ISoundSystem
{
	Vector3 ListenerPosition { get; }

	void SetListenerPosition(Vector3 position, Quaternion rotation);

	float GetMixerVolume(string name);

	void SetMixerVolume(string name, float level);

	void SetMasterVolume(float level);

	void SetMusicVolume(float level);

	void SetUIVolume(float level);

	void SetEnvironmentVolume(float level);

	void PlaySound2D(GameObject emitter, string soundName, int priority);

	void PlaySound2D(GameObject emitter, string soundName, int priority, float delay, Action callback);

	void PlaySound3D(GameObject emitter, string soundName, int priority);

	void PlaySound3D(GameObject emitter, string soundName, int priority, Action callback);

	void AddLimitedAreaSound(Transform parent, string soundName, int priority, int cutoffDistance, string customMixer);

	void AddAreaEmitter(Transform parent, GameObject emitter);

	void RemoveAreaEmitter(Transform parent, GameObject emitter);

	void AddLargeAreaSound(Transform parent, IEmitterMap emitterMap, string soundName, int priority, int cutoffDistance, string customMixer);

	void LoopSingle3DSound(GameObject emitter, string soundName, int priority);

	void LoopSingle3DSound(GameObject emitter, string soundName, int priority, Vector3 soundOffset);

	void LoopSingle2DSound(GameObject emitter, string soundName, int priority);

	void StopSound(GameObject emitter, string soundName);

	void SetCustomMixer(GameObject emitter, string soundName, string mixerName);

	void InvalidateSounds(GameObject emitter);
}

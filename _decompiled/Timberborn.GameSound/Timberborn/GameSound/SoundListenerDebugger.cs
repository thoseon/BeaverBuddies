using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;
using UnityEngine;

namespace Timberborn.GameSound;

internal class SoundListenerDebugger : ILoadableSingleton, ILateUpdatableSingleton
{
	private readonly ISoundSystem _soundSystem;

	private readonly RootObjectProvider _rootObjectProvider;

	private GameObject _parent;

	public SoundListenerDebugger(ISoundSystem soundSystem, RootObjectProvider rootObjectProvider)
	{
		_soundSystem = soundSystem;
		_rootObjectProvider = rootObjectProvider;
	}

	public void Load()
	{
		_parent = _rootObjectProvider.CreateRootObject("SoundListenerDebugger");
		Transform transform = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
		transform.SetParent(_parent.transform);
		transform.localScale = new Vector3(0.1f, 10f, 0.1f);
		transform.localPosition = new Vector3(0f, -10f, 0f);
		Object.Destroy(transform.GetComponent<Collider>());
		Transform transform2 = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
		transform2.SetParent(_parent.transform);
		transform2.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		Object.Destroy(transform2.GetComponent<Collider>());
		_parent.SetActive(value: false);
	}

	public void LateUpdateSingleton()
	{
		_parent.transform.position = _soundSystem.ListenerPosition;
	}

	public void ToggleActive()
	{
		_parent.SetActive(!_parent.activeSelf);
	}
}

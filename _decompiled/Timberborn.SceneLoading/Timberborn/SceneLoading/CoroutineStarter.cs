using System.Collections;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.SceneLoading;

internal class CoroutineStarter : ILoadableSingleton
{
	private class CoroutineStarterMonoBehaviour : MonoBehaviour
	{
	}

	private readonly RootObjectProvider _rootObjectProvider;

	private CoroutineStarterMonoBehaviour _monoBehaviour;

	public CoroutineStarter(RootObjectProvider rootObjectProvider)
	{
		_rootObjectProvider = rootObjectProvider;
	}

	public void Load()
	{
		GameObject gameObject = _rootObjectProvider.CreateRootObject("SceneLoader");
		Object.DontDestroyOnLoad(gameObject);
		_monoBehaviour = gameObject.AddComponent<CoroutineStarterMonoBehaviour>();
	}

	public void StartCoroutine(IEnumerator routine)
	{
		_monoBehaviour.StartCoroutine(routine);
	}
}

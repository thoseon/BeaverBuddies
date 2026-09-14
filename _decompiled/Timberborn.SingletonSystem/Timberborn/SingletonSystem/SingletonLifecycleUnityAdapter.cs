using Bindito.Core;
using UnityEngine;

namespace Timberborn.SingletonSystem;

internal class SingletonLifecycleUnityAdapter : MonoBehaviour
{
	private SingletonLifecycleService _singletonLifecycleService;

	[Inject]
	public void InjectDependencies(SingletonLifecycleService singletonLifecycleService)
	{
		_singletonLifecycleService = singletonLifecycleService;
	}

	public void Start()
	{
		_singletonLifecycleService.LoadAll();
	}

	public void OnDestroy()
	{
		_singletonLifecycleService.UnloadAll();
	}

	public void Update()
	{
		_singletonLifecycleService.UpdateAll();
	}

	public void LateUpdate()
	{
		_singletonLifecycleService.LateUpdateAll();
	}
}

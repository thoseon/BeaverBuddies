using UnityEngine;

namespace Timberborn.BaseComponentSystem;

internal class BaseComponentUnityAdapter : MonoBehaviour
{
	private bool _activated;

	private void OnEnable()
	{
		if (!_activated)
		{
			_activated = true;
			GetComponent<ComponentCache>().SetActive();
		}
	}
}

using Bindito.Core;
using UnityEngine;

namespace Timberborn.Autosaving;

internal class AutosaverUnityAdapter : MonoBehaviour
{
	private Autosaver _autosaver;

	[Inject]
	public void InjectDependencies(Autosaver autosaver)
	{
		_autosaver = autosaver;
	}

	public void OnApplicationQuit()
	{
		_autosaver.CreateExitSave();
	}
}

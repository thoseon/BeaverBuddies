using Bindito.Core;
using UnityEngine;

namespace Timberborn.GameSaveRuntimeSystem;

public class GameSaverUnityAdapter : MonoBehaviour
{
	private GameSaver _gameSaver;

	[Inject]
	public void InjectDependencies(GameSaver gameSaver)
	{
		_gameSaver = gameSaver;
	}

	public void LateUpdate()
	{
		_gameSaver.SaveQueued();
	}
}

using Bindito.Unity;
using Timberborn.AssetSystem;
using Timberborn.Coordinates;
using Timberborn.Debugging;
using Timberborn.InputSystem;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.CursorToolSystem;

public class CursorDebugger : ILoadableSingleton, IInputProcessor
{
	private static readonly string CrosshairMarkerPrefabPath = "UI/Markers/Debug/Crosshair";

	private static readonly string TileMarkerPrefabPath = "UI/Markers/Debug/Tile";

	private readonly CursorCoordinatesPicker _cursorCoordinatesPicker;

	private readonly InputService _inputService;

	private readonly DebugModeManager _debugModeManager;

	private readonly IAssetLoader _assetLoader;

	private readonly RootObjectProvider _rootObjectProvider;

	private Transform _crosshairMarker;

	private Transform _tileMarker;

	public Vector3 Position { get; private set; }

	public Vector3Int Coordinates { get; private set; }

	public bool Active { get; private set; }

	public CursorDebugger(CursorCoordinatesPicker cursorCoordinatesPicker, InputService inputService, DebugModeManager debugModeManager, IAssetLoader assetLoader, RootObjectProvider rootObjectProvider, IInstantiator instantiator)
	{
		_cursorCoordinatesPicker = cursorCoordinatesPicker;
		_inputService = inputService;
		_debugModeManager = debugModeManager;
		_assetLoader = assetLoader;
		_rootObjectProvider = rootObjectProvider;
	}

	public void Load()
	{
		Transform transform = _rootObjectProvider.CreateRootObject("CursorDebugger").transform;
		GameObject original = _assetLoader.Load<GameObject>(CrosshairMarkerPrefabPath);
		GameObject original2 = _assetLoader.Load<GameObject>(TileMarkerPrefabPath);
		_crosshairMarker = Object.Instantiate(original, transform).transform;
		_tileMarker = Object.Instantiate(original2, transform).transform;
		_inputService.AddInputProcessor(this);
		Hide();
	}

	public bool ProcessInput()
	{
		if (_debugModeManager.Enabled)
		{
			CursorCoordinates? cursorCoordinates = _cursorCoordinatesPicker.Pick();
			if (cursorCoordinates.HasValue)
			{
				CursorCoordinates valueOrDefault = cursorCoordinates.GetValueOrDefault();
				Show(valueOrDefault);
				goto IL_0039;
			}
		}
		Hide();
		goto IL_0039;
		IL_0039:
		return false;
	}

	private void Show(CursorCoordinates cursorCoordinates)
	{
		Position = CoordinateSystem.GridToWorld(cursorCoordinates.Coordinates);
		Coordinates = cursorCoordinates.TileCoordinates;
		_crosshairMarker.position = Position;
		_crosshairMarker.gameObject.SetActive(value: true);
		_tileMarker.position = CoordinateSystem.GridToWorldCentered(Coordinates);
		_tileMarker.gameObject.SetActive(value: true);
		Active = true;
	}

	private void Hide()
	{
		_crosshairMarker.gameObject.SetActive(value: false);
		_tileMarker.gameObject.SetActive(value: false);
		Active = false;
	}
}

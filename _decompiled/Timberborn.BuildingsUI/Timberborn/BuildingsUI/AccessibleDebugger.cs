using System.Collections.Generic;
using Timberborn.AssetSystem;
using Timberborn.CursorToolSystem;
using Timberborn.Debugging;
using Timberborn.Navigation;
using Timberborn.Rendering;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.BuildingsUI;

public class AccessibleDebugger : ILoadableSingleton, IUpdatableSingleton
{
	private static readonly string MarkerMeshPath = "Markers/DebuggingSphere";

	private static readonly string MarkerMaterialPath = "Markers/AccessibleDebuggerMarker";

	private readonly EventBus _eventBus;

	private readonly CursorDebugger _cursorDebugger;

	private readonly INavigationService _navigationService;

	private readonly MeshDrawerFactory _meshDrawerFactory;

	private readonly DebugModeManager _debugModeManager;

	private readonly IAssetLoader _assetLoader;

	private Accessible _selectedAccessible;

	private readonly List<PathCorner> _pathCorners = new List<PathCorner>();

	private MeshDrawer _meshDrawer;

	private bool _debugModeEnabled;

	public AccessibleDebugger(EventBus eventBus, CursorDebugger cursorDebugger, INavigationService navigationService, MeshDrawerFactory meshDrawerFactory, DebugModeManager debugModeManager, IAssetLoader assetLoader)
	{
		_eventBus = eventBus;
		_cursorDebugger = cursorDebugger;
		_navigationService = navigationService;
		_meshDrawerFactory = meshDrawerFactory;
		_debugModeManager = debugModeManager;
		_assetLoader = assetLoader;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_meshDrawer = _meshDrawerFactory.Create(_assetLoader.Load<Mesh>(MarkerMeshPath), _assetLoader.Load<Material>(MarkerMaterialPath));
		_debugModeEnabled = _debugModeManager.Enabled;
	}

	public void UpdateSingleton()
	{
		if (_debugModeEnabled && (bool)_selectedAccessible)
		{
			DrawSelectedAccessible();
		}
	}

	[OnEvent]
	public void OnDebugModeToggled(DebugModeToggledEvent debugModeToggledEvent)
	{
		_debugModeEnabled = debugModeToggledEvent.Enabled;
	}

	[OnEvent]
	public void OnSelectableObjectSelected(SelectableObjectSelectedEvent selectableObjectSelectedEvent)
	{
		_selectedAccessible = selectableObjectSelectedEvent.SelectableObject.GetEnabledComponent<Accessible>();
	}

	[OnEvent]
	public void OnSelectableObjectUnselected(SelectableObjectUnselectedEvent selectableObjectUnselectedEvent)
	{
		_selectedAccessible = null;
	}

	private void DrawSelectedAccessible()
	{
		Vector3 position = _cursorDebugger.Position;
		foreach (Vector3 access in _selectedAccessible.Accesses)
		{
			DrawAccessAndPath(access, position);
		}
	}

	private void DrawAccessAndPath(Vector3 accessPosition, Vector3 end)
	{
		if (_navigationService.DestinationIsReachable(accessPosition, end))
		{
			DrawPath(accessPosition, end);
			DrawAccess(accessPosition, Color.blue);
		}
		else
		{
			DrawAccess(accessPosition, Color.red);
		}
	}

	private void DrawAccess(Vector3 position, Color color)
	{
		_meshDrawer.DrawAtPosition(position, Quaternion.identity, color);
	}

	private void DrawPath(Vector3 start, Vector3 end)
	{
		if (_navigationService.FindPath(start, end, _pathCorners))
		{
			DrawPath(Color.blue);
		}
	}

	private void DrawPath(Color color)
	{
		for (int i = 0; i < _pathCorners.Count - 1; i++)
		{
			PathCorner pathCorner = _pathCorners[i];
			PathCorner pathCorner2 = _pathCorners[i + 1];
			Debug.DrawLine(pathCorner.Position, pathCorner2.Position, color, 0f, depthTest: false);
		}
	}
}

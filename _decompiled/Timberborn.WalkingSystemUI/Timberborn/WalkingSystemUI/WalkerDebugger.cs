using System.Collections.Generic;
using System.Linq;
using Timberborn.AssetSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.BlueprintSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.Debugging;
using Timberborn.Navigation;
using Timberborn.RootProviders;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.WalkingSystem;
using UnityEngine;

namespace Timberborn.WalkingSystemUI;

public class WalkerDebugger : ILoadableSingleton, ILateUpdatableSingleton
{
	private readonly EventBus _eventBus;

	private readonly DebugModeManager _debugModeManager;

	private readonly IAssetLoader _assetLoader;

	private readonly RootObjectProvider _rootObjectProvider;

	private readonly ISpecService _specService;

	private bool _walkerSelected;

	private Walker _walker;

	private CharacterModel _characterModel;

	private readonly List<GameObject> _cornerMarkers = new List<GameObject>();

	private Vector3? _destination;

	private GameObject _walkerGameObjectMarker;

	private GameObject _walkerModelMarker;

	private GameObject _destinationMarker;

	private GameObject _cornerMarkerPrefab;

	private GameObject _root;

	private Vector3 Destination
	{
		get
		{
			if (_walker.PathCorners.IsEmpty())
			{
				return _root.transform.position;
			}
			return _walker.PathCorners.Last().Position;
		}
	}

	public WalkerDebugger(EventBus eventBus, DebugModeManager debugModeManager, IAssetLoader assetLoader, RootObjectProvider rootObjectProvider, ISpecService specService)
	{
		_eventBus = eventBus;
		_debugModeManager = debugModeManager;
		_assetLoader = assetLoader;
		_rootObjectProvider = rootObjectProvider;
		_specService = specService;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_root = _rootObjectProvider.CreateRootObject("WalkerDebugger");
		WalkerDebuggerSpec singleSpec = _specService.GetSingleSpec<WalkerDebuggerSpec>();
		_walkerGameObjectMarker = CreateMarker(singleSpec.WalkerGameObjectMarkerPath);
		_walkerModelMarker = CreateMarker(singleSpec.WalkerModelMarkerPath);
		_destinationMarker = CreateMarker(singleSpec.DestinationMarkerPath);
		_cornerMarkerPrefab = _assetLoader.Load<GameObject>(singleSpec.CornerMarkerPath);
	}

	public void LateUpdateSingleton()
	{
		if (_debugModeManager.Enabled && _walkerSelected)
		{
			UpdateMarkers();
		}
		else
		{
			HideMarkers();
		}
	}

	[OnEvent]
	public void OnSelectableObjectSelected(SelectableObjectSelectedEvent selectableObjectSelectedEvent)
	{
		Walker component = selectableObjectSelectedEvent.SelectableObject.GetComponent<Walker>();
		if ((bool)(BaseComponent)(object)component)
		{
			UpdateSelectedWalker(component);
		}
	}

	[OnEvent]
	public void OnSelectableObjectUnselected(SelectableObjectUnselectedEvent selectableObjectUnselectedEvent)
	{
		HideMarkers();
		_walkerSelected = false;
	}

	private GameObject CreateMarker(string name)
	{
		return Object.Instantiate(_assetLoader.Load<GameObject>(name), _root.transform);
	}

	private void UpdateSelectedWalker(Walker walker)
	{
		_walker = walker;
		_characterModel = ((BaseComponent)(object)_walker).GetComponent<CharacterModel>();
		_walkerSelected = true;
	}

	private void UpdateMarkers()
	{
		UpdateMarker();
		if (PathMarkersStale())
		{
			ResetPathMarkers();
		}
	}

	private void UpdateMarker()
	{
		Transform transform = ((BaseComponent)(object)_walker).Transform;
		_walkerGameObjectMarker.transform.SetPositionAndRotation(transform.position, transform.rotation);
		_walkerGameObjectMarker.SetActive(value: true);
		_walkerModelMarker.transform.SetPositionAndRotation(_characterModel.Position, _characterModel.Rotation);
		_walkerModelMarker.SetActive(value: true);
	}

	private bool PathMarkersStale()
	{
		if (!(_destination != Destination))
		{
			return _walker.PathCorners.Count != _cornerMarkers.Count;
		}
		return true;
	}

	private void HideMarkers()
	{
		_walkerGameObjectMarker.SetActive(value: false);
		_walkerModelMarker.SetActive(value: false);
		_destinationMarker.SetActive(value: false);
		foreach (GameObject cornerMarker in _cornerMarkers)
		{
			Object.Destroy(cornerMarker);
		}
		_cornerMarkers.Clear();
		_destination = null;
	}

	private void ResetPathMarkers()
	{
		HideMarkers();
		_destinationMarker.transform.position = Destination;
		_destinationMarker.SetActive(value: true);
		foreach (PathCorner pathCorner in _walker.PathCorners)
		{
			Vector3 position = pathCorner.Position + Random.insideUnitSphere * 0.1f;
			_cornerMarkers.Add(Object.Instantiate(_cornerMarkerPrefab, position, Quaternion.identity, _root.transform));
		}
		_destination = Destination;
	}
}

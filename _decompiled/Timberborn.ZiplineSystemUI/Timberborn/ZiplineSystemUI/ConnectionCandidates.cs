using System.Collections.Generic;
using Timberborn.AssetSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using Timberborn.Navigation;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.ZiplineSystem;
using UnityEngine;

namespace Timberborn.ZiplineSystemUI;

internal class ConnectionCandidates : ILoadableSingleton, IUpdatableSingleton, ISingletonPreviewNavMeshListener, ISingletonInstantNavMeshListener
{
	private readonly ZiplineTowerRegistry _ziplineTowerRegistry;

	private readonly ZiplineConnectionService _ziplineConnectionService;

	private readonly IAssetLoader _assetLoader;

	private readonly ISpecService _specService;

	private readonly Highlighter _highlighter;

	private ZiplineSystemColorsSpec _ziplineSystemColorsSpec;

	private GameObject _markerPrefab;

	private readonly List<ZiplineTower> _candidates = new List<ZiplineTower>();

	private readonly List<GameObject> _markers = new List<GameObject>();

	private ZiplineTower _origin;

	private bool _enabled;

	private bool _shouldUpdateCandidates;

	private bool _drawMarkers;

	public ConnectionCandidates(ZiplineTowerRegistry ziplineTowerRegistry, ZiplineConnectionService ziplineConnectionService, IAssetLoader assetLoader, ISpecService specService, Highlighter highlighter)
	{
		_ziplineTowerRegistry = ziplineTowerRegistry;
		_ziplineConnectionService = ziplineConnectionService;
		_assetLoader = assetLoader;
		_specService = specService;
		_highlighter = highlighter;
	}

	public void Load()
	{
		_ziplineSystemColorsSpec = _specService.GetSingleSpec<ZiplineSystemColorsSpec>();
		_markerPrefab = _assetLoader.Load<GameObject>("Markers/ZiplineMarker");
	}

	public void EnableAndDrawMarkers(ZiplineTower origin)
	{
		EnableInternal(origin, drawMarkers: true);
	}

	public void Enable(ZiplineTower origin)
	{
		EnableInternal(origin, drawMarkers: false);
	}

	public void Disable()
	{
		_highlighter.UnhighlightAllPrimary();
		_origin = null;
		ClearCandidates();
		_enabled = false;
		_shouldUpdateCandidates = false;
	}

	public bool Contains(ZiplineTower ziplineTower)
	{
		return _candidates.Contains(ziplineTower);
	}

	public void UpdateSingleton()
	{
		if (_enabled && _shouldUpdateCandidates)
		{
			UpdateCandidates();
			_shouldUpdateCandidates = false;
		}
	}

	public void OnInstantNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		_shouldUpdateCandidates = true;
	}

	public void OnPreviewNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		_shouldUpdateCandidates = true;
	}

	public void UpdateCandidates()
	{
		ClearCandidates();
		AddCandidates();
	}

	private void EnableInternal(ZiplineTower origin, bool drawMarkers)
	{
		_origin = origin;
		_shouldUpdateCandidates = true;
		_enabled = true;
		_highlighter.HighlightPrimary(_origin, _ziplineSystemColorsSpec.OriginColor);
		_drawMarkers = drawMarkers;
	}

	private void ClearCandidates()
	{
		_candidates.Clear();
		foreach (GameObject marker in _markers)
		{
			Object.Destroy(marker.gameObject);
		}
		_markers.Clear();
	}

	private void AddCandidates()
	{
		foreach (ZiplineTower ziplineTower in _ziplineTowerRegistry.ZiplineTowers)
		{
			if (_ziplineConnectionService.CanBeConnected(_origin, ziplineTower))
			{
				_candidates.Add(ziplineTower);
				if (_drawMarkers)
				{
					CreateMarker(ziplineTower);
				}
			}
		}
	}

	private void CreateMarker(ZiplineTower ziplineTower)
	{
		Vector3 vector = CoordinateSystem.GridToWorld(ziplineTower.CableAnchorPoint);
		Vector3 forward = vector - CoordinateSystem.GridToWorld(_origin.CableAnchorPoint);
		forward.y = 0f;
		Quaternion rotation = Quaternion.LookRotation(forward, Vector3.up);
		GameObject item = Object.Instantiate(_markerPrefab, vector + new Vector3(0f, 0.12f, 0f), rotation);
		_markers.Add(item);
	}
}

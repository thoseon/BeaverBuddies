using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Rendering;
using Timberborn.SelectionSystem;
using Timberborn.WaterBuildings;
using UnityEngine;

namespace Timberborn.WaterBuildingsUI;

internal class FillValveMarker : BaseComponent, IAwakableComponent, IUpdatableComponent, ISelectionListener
{
	private static readonly Color32 ActiveMarkerColor = Color.blue;

	private static readonly Color32 InactiveMarkerColor = Color.blue * 0.65f;

	private static readonly float MarkerYOffset = 0.02f;

	private readonly MarkerDrawerFactory _markerDrawerFactory;

	private FillValve _fillValve;

	private MeshDrawer _activeMarkerDrawer;

	private MeshDrawer _inactiveMarkerDrawer;

	private BlockObject _blockObject;

	public FillValveMarker(MarkerDrawerFactory markerDrawerFactory)
	{
		_markerDrawerFactory = markerDrawerFactory;
	}

	public void Awake()
	{
		_fillValve = GetComponent<FillValve>();
		_blockObject = GetComponent<BlockObject>();
		_activeMarkerDrawer = _markerDrawerFactory.CreateTileDrawer(ActiveMarkerColor);
		_inactiveMarkerDrawer = _markerDrawerFactory.CreateTileDrawer(InactiveMarkerColor);
		DisableComponent();
	}

	public void Update()
	{
		Vector3Int outputCoordinates = _fillValve.OutputCoordinates;
		Vector3Int coordinates = new Vector3Int(outputCoordinates.x, outputCoordinates.y, 0);
		if (_fillValve.TargetHeightEnabled)
		{
			((!_fillValve.IsAutomated || !_fillValve.IsInputOn) ? _activeMarkerDrawer : _inactiveMarkerDrawer).DrawAtCoordinates(coordinates, _fillValve.ClampedTargetHeight + MarkerYOffset);
		}
		if (_fillValve.IsAutomated && _fillValve.AutomationTargetHeightEnabled)
		{
			(_fillValve.IsInputOn ? _activeMarkerDrawer : _inactiveMarkerDrawer).DrawAtCoordinates(coordinates, _fillValve.ClampedAutomationTargetHeight + MarkerYOffset);
		}
	}

	public void OnSelect()
	{
		if (!_blockObject.IsPreview)
		{
			EnableComponent();
		}
	}

	public void OnUnselect()
	{
		DisableComponent();
	}
}

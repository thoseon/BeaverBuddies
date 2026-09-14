using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Rendering;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.AutomationBuildingsUI;

internal class DepthSensorMarker : BaseComponent, IAwakableComponent, IUpdatableComponent, ISelectionListener
{
	private static readonly Color32 MarkerColor = Color.blue;

	private static readonly float MarkerYOffset = 0.02f;

	private readonly MarkerDrawerFactory _markerDrawerFactory;

	private DepthSensor _depthSensor;

	private MeshDrawer _markerDrawer;

	private BlockObject _blockObject;

	public DepthSensorMarker(MarkerDrawerFactory markerDrawerFactory)
	{
		_markerDrawerFactory = markerDrawerFactory;
	}

	public void Awake()
	{
		_depthSensor = GetComponent<DepthSensor>();
		_blockObject = GetComponent<BlockObject>();
		_markerDrawer = _markerDrawerFactory.CreateTileDrawer(MarkerColor);
		DisableComponent();
	}

	public void Update()
	{
		Vector3Int sensorCoordinates = _depthSensor.SensorCoordinates;
		_markerDrawer.DrawAtCoordinates(new Vector3Int(sensorCoordinates.x, sensorCoordinates.y, 0), _depthSensor.Threshold + MarkerYOffset);
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

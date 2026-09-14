using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.Rendering;
using UnityEngine;

namespace Timberborn.WaterBuildingsUI;

internal class WaterDirectionPreviewMarker : BaseComponent, IUpdatableComponent, IAwakableComponent, IPreviewSelectionListener, IPostPlacementChangeListener
{
	private readonly MarkerDrawerFactory _markerDrawerFactory;

	private MeshDrawer _meshDrawer;

	private BlockObject _blockObject;

	private Matrix4x4 _marker;

	public WaterDirectionPreviewMarker(MarkerDrawerFactory markerDrawerFactory)
	{
		_markerDrawerFactory = markerDrawerFactory;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_meshDrawer = _markerDrawerFactory.CreateArrowMarkerDrawer();
		DisableComponent();
	}

	public void Update()
	{
		_meshDrawer.Draw(_marker);
	}

	public void OnPreviewSelect()
	{
		EnableComponent();
	}

	public void OnPreviewUnselect()
	{
		DisableComponent();
	}

	public void OnPostPlacementChanged()
	{
		Quaternion q = Quaternion.AngleAxis(90f + _blockObject.Orientation.ToAngle(), Vector3.up);
		_marker = Matrix4x4.TRS(GetPosition(), q, Vector3.one);
	}

	private Vector3 GetPosition()
	{
		return CoordinateSystem.GridToWorld(new Vector3((float)_blockObject.Coordinates.x + 0.5f, (float)_blockObject.Coordinates.y + 0.5f, (float)_blockObject.Coordinates.z + 1.05f));
	}
}

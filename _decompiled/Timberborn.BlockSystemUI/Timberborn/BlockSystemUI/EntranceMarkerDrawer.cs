using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.Rendering;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.BlockSystemUI;

public class EntranceMarkerDrawer : BaseComponent, IAwakableComponent, IInitializablePreview, IInitializableEntity, ILateUpdatableComponent, ISelectionListener, IPreviewSelectionListener
{
	private static readonly float EntranceMarkerYOffset = 0.2f;

	private readonly MarkerDrawerFactory _markerDrawerFactory;

	private BlockObject _blockObject;

	private BlockObjectModelController _blockObjectModelController;

	private MeshDrawer _entranceMarkerMeshDrawer;

	public EntranceMarkerDrawer(MarkerDrawerFactory markerDrawerFactory)
	{
		_markerDrawerFactory = markerDrawerFactory;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_blockObjectModelController = GetComponent<BlockObjectModelController>();
		DisableComponent();
	}

	public void InitializePreview()
	{
		_entranceMarkerMeshDrawer = _markerDrawerFactory.CreateEntranceMarkerDrawer();
	}

	public void InitializeEntity()
	{
		_entranceMarkerMeshDrawer = _markerDrawerFactory.CreateEntranceMarkerDrawer();
	}

	public void LateUpdate()
	{
		BlockObjectModelController blockObjectModelController = _blockObjectModelController;
		if ((blockObjectModelController == null || blockObjectModelController.IsAnyModelShown) && _blockObject.HasEntrance)
		{
			DrawEntrance();
		}
	}

	public void OnSelect()
	{
		EnableComponent();
	}

	public void OnUnselect()
	{
		DisableComponent();
	}

	public void OnPreviewSelect()
	{
		EnableComponent();
	}

	public void OnPreviewUnselect()
	{
		DisableComponent();
	}

	private void DrawEntrance()
	{
		PositionedEntrance positionedEntrance = _blockObject.PositionedEntrance;
		Vector3Int coordinates = positionedEntrance.Coordinates;
		Quaternion rotation = Quaternion.AngleAxis(positionedEntrance.Direction2D.Across().ToAngle(), Vector3.up);
		_entranceMarkerMeshDrawer.DrawAtCoordinates(coordinates, EntranceMarkerYOffset, rotation);
	}
}

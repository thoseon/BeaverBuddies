using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using Timberborn.GameDistricts;
using Timberborn.GameDistrictsMigration;
using Timberborn.GameDistrictsMigrationBatchControl;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.GameDistrictsUI;

internal class DistrictConnectionDrawingService : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private readonly DistrictConnectionLineRenderer _districtConnectionLineRenderer;

	private readonly EventBus _eventBus;

	private readonly Highlighter _highlighter;

	private readonly ManualMigrationDistrictSetter _manualMigrationDistrictSetter;

	private bool _enabled;

	private bool _districtsConnected;

	private Color _connectionHighlightColor;

	private bool ShouldDraw
	{
		get
		{
			if (_enabled && _districtsConnected)
			{
				return _manualMigrationDistrictSetter.AreDistrictsSet;
			}
			return false;
		}
	}

	public DistrictConnectionDrawingService(ISpecService specService, DistrictConnectionLineRenderer districtConnectionLineRenderer, EventBus eventBus, Highlighter highlighter, ManualMigrationDistrictSetter manualMigrationDistrictSetter)
	{
		_specService = specService;
		_districtConnectionLineRenderer = districtConnectionLineRenderer;
		_eventBus = eventBus;
		_highlighter = highlighter;
		_manualMigrationDistrictSetter = manualMigrationDistrictSetter;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_connectionHighlightColor = _specService.GetSingleSpec<DistrictConnectionDrawingServiceSpec>().ConnectionHighlight;
	}

	[OnEvent]
	public void OnManualMigrationPanelOpened(ManualMigrationPanelOpenedEvent manualMigrationPanelOpenedEvent)
	{
		_enabled = true;
		DrawOrClearConnection();
	}

	[OnEvent]
	public void OnManualMigrationPanelClosed(ManualMigrationPanelClosedEvent manualMigrationPanelClosedEvent)
	{
		_enabled = false;
		Clear();
	}

	[OnEvent]
	public void OnManualMigrationBlockingStateChanged(ManualMigrationBlockingStateChangedEvent manualMigrationBlockingStateChangedEvent)
	{
		_districtsConnected = manualMigrationBlockingStateChangedEvent.IsEnabled;
		DrawOrClearConnection();
	}

	private void DrawOrClearConnection()
	{
		if (ShouldDraw)
		{
			DrawConnection();
			HighlightDistricts();
		}
		else
		{
			Clear();
		}
	}

	private void DrawConnection()
	{
		Vector3 connectionPoint = GetConnectionPoint(_manualMigrationDistrictSetter.LeftDistrict);
		Vector3 connectionPoint2 = GetConnectionPoint(_manualMigrationDistrictSetter.RightDistrict);
		_districtConnectionLineRenderer.BuildMesh(connectionPoint, connectionPoint2);
	}

	private static Vector3 GetConnectionPoint(DistrictCenter districtCenter)
	{
		Vector3 coordinates = CoordinateSystem.WorldToGrid(districtCenter.GetComponent<ConnectionAnchorPointSpec>().Position);
		return CoordinateSystem.GridToWorld(districtCenter.GetComponent<BlockObject>().TransformCoordinates(coordinates));
	}

	private void HighlightDistricts()
	{
		_highlighter.UnhighlightAllSecondary();
		_highlighter.HighlightSecondary(_manualMigrationDistrictSetter.LeftDistrict, _connectionHighlightColor);
		_highlighter.HighlightSecondary(_manualMigrationDistrictSetter.RightDistrict, _connectionHighlightColor);
	}

	private void Clear()
	{
		_districtConnectionLineRenderer.Clear();
		_highlighter.UnhighlightAllSecondary();
	}
}

using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.CameraSystem;
using Timberborn.Coordinates;
using Timberborn.Localization;
using Timberborn.NotificationSystem;
using Timberborn.SelectionSystem;
using Timberborn.StartingLocationSystem;

namespace Timberborn.GameStartup;

internal class StartingBuildingInitializer
{
	private static readonly float ZoomIncreasePerLevel = 0.1f;

	private static readonly float StartingVerticalCameraAngle = 60f;

	private static readonly float StartingHorizontalCameraAngleOffset = 35f;

	private static readonly string NewGameLocKey = "NewGame.Notification";

	private readonly StartingLocationService _startingLocationService;

	private readonly CameraService _cameraService;

	private readonly StartingBuildingSpawner _startingBuildingSpawner;

	private readonly CameraTargeter _cameraTargeter;

	private readonly NotificationBus _notificationBus;

	private readonly ILoc _loc;

	public Placement? InitialPlacement { get; private set; }

	public StartingBuildingInitializer(StartingLocationService startingLocationService, CameraService cameraService, StartingBuildingSpawner startingBuildingSpawner, CameraTargeter cameraTargeter, NotificationBus notificationBus, ILoc loc)
	{
		_startingLocationService = startingLocationService;
		_cameraService = cameraService;
		_startingBuildingSpawner = startingBuildingSpawner;
		_cameraTargeter = cameraTargeter;
		_notificationBus = notificationBus;
		_loc = loc;
	}

	public void Initialize()
	{
		if (_startingLocationService.HasStartingLocation())
		{
			InitialPlacement = _startingLocationService.GetPlacement();
		}
		_startingBuildingSpawner.Place(InitialPlacement);
		SetCamera();
		_startingLocationService.DeleteStartingLocations();
		Notify();
	}

	private void SetCamera()
	{
		Building startingBuilding = _startingBuildingSpawner.StartingBuilding;
		if (startingBuilding != null)
		{
			_cameraService.VerticalAngle = StartingVerticalCameraAngle;
			BlockObject component = startingBuilding.GetComponent<BlockObject>();
			_cameraService.HorizontalAngle = component.Orientation.ToAngle() + StartingHorizontalCameraAngleOffset;
			_cameraService.ZoomLevel = (float)component.Coordinates.z * ZoomIncreasePerLevel;
			_cameraTargeter.CenterCameraOn(startingBuilding.GetComponent<SelectableObject>());
		}
	}

	private void Notify()
	{
		Building startingBuilding = _startingBuildingSpawner.StartingBuilding;
		if (startingBuilding != null)
		{
			_notificationBus.Post(_loc.T(NewGameLocKey), startingBuilding);
		}
	}
}

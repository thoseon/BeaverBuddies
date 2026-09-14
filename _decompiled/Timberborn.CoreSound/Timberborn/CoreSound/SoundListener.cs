using Timberborn.BlockObjectPickingSystem;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.CameraSystem;
using Timberborn.Coordinates;
using Timberborn.GridTraversing;
using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;
using Timberborn.TerrainQueryingSystem;
using UnityEngine;

namespace Timberborn.CoreSound;

internal class SoundListener : ILoadableSingleton, ILateUpdatableSingleton
{
	private readonly TerrainPicker _terrainPicker;

	private readonly CameraService _cameraService;

	private readonly ISoundSystem _soundSystem;

	private readonly BlockObjectRaycaster _blockObjectRaycaster;

	private readonly ISpecService _specService;

	private int _maxVerticalListenerPositionAboveGround;

	public SoundListener(TerrainPicker terrainPicker, CameraService cameraService, ISoundSystem soundSystem, BlockObjectRaycaster blockObjectRaycaster, ISpecService specService)
	{
		_terrainPicker = terrainPicker;
		_cameraService = cameraService;
		_soundSystem = soundSystem;
		_blockObjectRaycaster = blockObjectRaycaster;
		_specService = specService;
	}

	public void Load()
	{
		CoreSoundSpec singleSpec = _specService.GetSingleSpec<CoreSoundSpec>();
		_maxVerticalListenerPositionAboveGround = singleSpec.MaxVerticalListenerPositionAboveGround;
	}

	public void LateUpdateSingleton()
	{
		if (TryGetScreenCenterPosition(out var position))
		{
			Quaternion rotation = GetRotation();
			position = Vector3.Lerp(_soundSystem.ListenerPosition, position, 0.1f);
			_soundSystem.SetListenerPosition(position, rotation);
		}
	}

	private bool TryGetScreenCenterPosition(out Vector3 position)
	{
		Vector2 screenPoint = new Vector2(Screen.width, Screen.height) * 0.5f;
		Ray ray = _cameraService.ScreenPointToRayInGridSpace(screenPoint);
		TraversedCoordinates? traversedCoordinates = _terrainPicker.PickTerrainCoordinates(ray);
		bool flag = _blockObjectRaycaster.TryHitBlockObject<BlockObject>(ray, out var blockObjectHit);
		if (traversedCoordinates.HasValue)
		{
			float y = (float)_maxVerticalListenerPositionAboveGround * _cameraService.NormalizedDefaultZoomLevel;
			Vector3 intersection = traversedCoordinates.Value.Intersection;
			Vector3 position2 = _cameraService.Transform.position;
			position = CoordinateSystem.GridToWorld(intersection) + new Vector3(0f, y, 0f);
			if (flag)
			{
				Vector3 vector = CoordinateSystem.GridToWorld(blockObjectHit.HitBlock.Coordinates);
				if (Vector3.Distance(position, position2) > Vector3.Distance(vector, position2))
				{
					position = vector;
				}
			}
			return true;
		}
		position = (flag ? CoordinateSystem.GridToWorld(blockObjectHit.HitBlock.Coordinates) : default(Vector3));
		return flag;
	}

	private Quaternion GetRotation()
	{
		return Quaternion.AngleAxis(_cameraService.Transform.rotation.eulerAngles.y, Vector3.up);
	}
}

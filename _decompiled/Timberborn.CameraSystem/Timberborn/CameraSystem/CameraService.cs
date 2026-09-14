using System;
using Timberborn.BlueprintSystem;
using Timberborn.CameraSettingsSystem;
using Timberborn.Coordinates;
using Timberborn.MapStateSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CameraSystem;

public class CameraService : ISaveableSingleton, ILoadableSingleton, ILateUpdatableSingleton
{
	private static readonly SingletonKey CameraServiceKey = new SingletonKey("CameraService");

	private static readonly PropertyKey<CameraState> CameraStateKey = new PropertyKey<CameraState>("CameraState");

	private readonly ISingletonLoader _singletonLoader;

	private readonly MapSize _mapSize;

	private readonly MapEditorMode _mapEditorMode;

	private readonly CameraStateSerializer _cameraStateSerializer;

	private readonly ISpecService _specService;

	private readonly CameraFactory _cameraFactory;

	private readonly CameraSettings _cameraSettings;

	private Camera _camera;

	private Vector3 _lastFramePosition;

	private Quaternion _lastFrameRotation;

	private CameraServiceSpec _cameraServiceSpec;

	public Vector3 Target { get; private set; }

	public float VerticalAngle { get; set; }

	public float HorizontalAngle { get; set; }

	public float ZoomLevel { get; set; }

	public bool FreeMode { get; set; }

	public Vector3 OffsetFromTarget => Rotation * Vector3.back * DistanceFromTarget;

	public Matrix4x4 ProjectionMatrix => _camera.projectionMatrix;

	public Transform Transform => _camera.transform;

	public float NormalizedDefaultZoomLevel
	{
		get
		{
			FloatLimitsSpec defaultZoomLimits = _cameraServiceSpec.DefaultZoomLimits;
			return Mathf.Clamp01((ZoomLevel - defaultZoomLimits.Min) / (defaultZoomLimits.Max - defaultZoomLimits.Min));
		}
	}

	public float ZoomSpeedScale
	{
		get
		{
			if (ZoomLevel < 0f)
			{
				return 1f - Mathf.Abs(ZoomLevel / -9f);
			}
			if (ZoomLevel > 0f)
			{
				return Math.Max(ZoomLevel / 2f, 1f);
			}
			return 1f;
		}
	}

	public float FieldOfView
	{
		get
		{
			return _camera.fieldOfView;
		}
		set
		{
			_camera.fieldOfView = value;
		}
	}

	public float NearClipPlane
	{
		get
		{
			return _camera.nearClipPlane;
		}
		set
		{
			_camera.nearClipPlane = value;
		}
	}

	public Quaternion FacingCamera
	{
		get
		{
			if ((bool)_camera)
			{
				return Quaternion.AngleAxis(_camera.transform.eulerAngles.y, Vector3.up);
			}
			return Quaternion.identity;
		}
	}

	private float DistanceFromTarget => Mathf.Pow(_cameraServiceSpec.ZoomBase, ZoomLevel) * _cameraServiceSpec.BaseDistance;

	private Quaternion Rotation => Quaternion.AngleAxis(HorizontalAngle, Vector3.up) * Quaternion.AngleAxis(VerticalAngle, Vector3.right);

	private FloatLimitsSpec ZoomLimitsSpec
	{
		get
		{
			if (FreeMode)
			{
				return _cameraServiceSpec.FreeModeZoomLimits;
			}
			if (_cameraSettings.UnlockZoom)
			{
				return _cameraServiceSpec.UnlockedZoomLimits;
			}
			if (_mapEditorMode.IsMapEditor)
			{
				return _cameraServiceSpec.MapEditorZoomLimits;
			}
			return _cameraServiceSpec.DefaultZoomLimits;
		}
	}

	public event EventHandler BeforeCameraUpdate;

	public event EventHandler CameraPositionOrRotationChanged;

	public CameraService(ISingletonLoader singletonLoader, MapSize mapSize, MapEditorMode mapEditorMode, CameraStateSerializer cameraStateSerializer, ISpecService specService, CameraFactory cameraFactory, CameraSettings cameraSettings)
	{
		_singletonLoader = singletonLoader;
		_mapSize = mapSize;
		_mapEditorMode = mapEditorMode;
		_cameraStateSerializer = cameraStateSerializer;
		_specService = specService;
		_cameraFactory = cameraFactory;
		_cameraSettings = cameraSettings;
	}

	public void LateUpdateSingleton()
	{
		BeforeCameraUpdate?.Invoke(this, EventArgs.Empty);
		UpdatePositionAndRotation();
		CheckPositionOrRotationChange();
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			singletonSaver.GetSingleton(CameraServiceKey).Set(CameraStateKey, GetCurrentState(), _cameraStateSerializer);
		}
	}

	public void Load()
	{
		_cameraServiceSpec = _specService.GetSingleSpec<CameraServiceSpec>();
		_camera = _cameraFactory.Create("CameraService");
		VerticalAngle = _cameraServiceSpec.VerticalAngle;
		HorizontalAngle = _cameraServiceSpec.HorizontalAngle;
		ZoomLevel = _cameraServiceSpec.ZoomLevel;
		if (_singletonLoader.TryGetSingleton(CameraServiceKey, out var objectLoader))
		{
			CameraState cameraState = objectLoader.Get(CameraStateKey, _cameraStateSerializer);
			RestoreState(cameraState);
		}
		UpdatePositionAndRotation();
	}

	public void MoveCameraBy(Vector3 delta)
	{
		Vector3 vector = Quaternion.AngleAxis(HorizontalAngle, Vector3.up) * delta;
		MoveTargetTo(Target + vector);
	}

	public void MoveTargetTo(Vector3 point)
	{
		Vector3 coordinates = CoordinateSystem.WorldToGrid(point);
		Target = CoordinateSystem.GridToWorld(ClampTarget(coordinates));
	}

	public void SetZoomLevel(float distanceFromTarget, float delta)
	{
		float baseDistance = _cameraServiceSpec.BaseDistance;
		float zoomBase = _cameraServiceSpec.ZoomBase;
		float num = Mathf.Log(distanceFromTarget / baseDistance) / Mathf.Log(zoomBase);
		ZoomLevel = Mathf.Clamp(num + delta, ZoomLimitsSpec.Min, ZoomLimitsSpec.Max);
	}

	public void ModifyZoomLevel(float scroll)
	{
		ZoomLevel += GetZoomDelta(ZoomLevel, scroll, 0.5f);
	}

	public void ModifyHorizontalAngle(float delta)
	{
		HorizontalAngle += delta;
	}

	public void ModifyVerticalAngle(float delta)
	{
		float num = VerticalAngle + delta;
		VerticalAngle = (FreeMode ? num : Mathf.Clamp(num, _cameraServiceSpec.VerticalAngleLimits.Min, _cameraServiceSpec.VerticalAngleLimits.Max));
	}

	public Vector3 WorldSpaceToPanelSpace(VisualElement panel, Vector3 position)
	{
		return RuntimePanelUtils.CameraTransformWorldToPanel(panel.panel, position, _camera);
	}

	public Ray ScreenPointToPreciseRayInWorldSpace(Vector2 screenPoint)
	{
		Vector2 vector = new Vector2(screenPoint.x / (float)Screen.width, screenPoint.y / (float)Screen.height);
		float x = vector.x * 2f - 1f;
		float y = vector.y * 2f - 1f;
		Vector4 vector2 = new Vector4(x, y, -1f, 1f);
		Vector4 vector3 = _camera.projectionMatrix.inverse * vector2;
		vector3 = new Vector4(vector3.x, vector3.y, -1f, 0f);
		Vector4 normalized = (_camera.cameraToWorldMatrix * vector3).normalized;
		return new Ray(_camera.transform.position, normalized);
	}

	public Ray ScreenPointToRayInWorldSpace(Vector2 screenPoint)
	{
		return _camera.ScreenPointToRay(screenPoint);
	}

	public Ray ScreenPointToRayInGridSpace(Vector2 screenPoint)
	{
		return CoordinateSystem.WorldToGrid(ScreenPointToRayInWorldSpace(screenPoint));
	}

	public CameraState GetCurrentState()
	{
		return new CameraState(Target, ZoomLevel, HorizontalAngle, VerticalAngle);
	}

	public void RestoreState(CameraState cameraState)
	{
		MoveTargetTo(cameraState.Target);
		ZoomLevel = cameraState.ZoomLevel;
		HorizontalAngle = cameraState.HorizontalAngle;
		VerticalAngle = cameraState.VerticalAngle;
	}

	public void SetProjectionMatrix(Matrix4x4 projectionMatrix)
	{
		_camera.projectionMatrix = projectionMatrix;
		CameraPositionOrRotationChanged?.Invoke(this, EventArgs.Empty);
	}

	public void ResetProjectionMatrix()
	{
		_camera.ResetProjectionMatrix();
		CameraPositionOrRotationChanged?.Invoke(this, EventArgs.Empty);
	}

	public bool IsInFront(Vector3 point)
	{
		Vector3 forward = _camera.transform.forward;
		Vector3 to = point - _camera.transform.position;
		return Vector3.Angle(forward, to) < 90f;
	}

	private void UpdatePositionAndRotation()
	{
		Vector3 position = Target + OffsetFromTarget;
		_camera.transform.SetPositionAndRotation(position, Rotation);
	}

	private void CheckPositionOrRotationChange()
	{
		if (!_lastFramePosition.Equals(_camera.transform.position) || !_lastFrameRotation.Equals(_camera.transform.rotation))
		{
			CameraPositionOrRotationChanged?.Invoke(this, EventArgs.Empty);
		}
		_camera.transform.GetPositionAndRotation(out _lastFramePosition, out _lastFrameRotation);
	}

	private float GetZoomDelta(float zoomLevel, float scrollDelta, float zoomDeltaLimit)
	{
		return Math.Min(Mathf.Clamp(zoomLevel - scrollDelta * _cameraServiceSpec.ZoomSpeed, ZoomLimitsSpec.Min, ZoomLimitsSpec.Max) - zoomLevel, zoomDeltaLimit);
	}

	private Vector3 ClampTarget(Vector3 coordinates)
	{
		Vector3Int terrainSize = _mapSize.TerrainSize;
		float num = (FreeMode ? _cameraServiceSpec.FreeModeMapMargin : _cameraServiceSpec.MapMargin);
		return new Vector3(Mathf.Clamp(coordinates.x, 0f - num, (float)terrainSize.x + num), Mathf.Clamp(coordinates.y, 0f - num, (float)terrainSize.y + num), Mathf.Clamp(coordinates.z, 0f - num, (float)terrainSize.z + num));
	}
}

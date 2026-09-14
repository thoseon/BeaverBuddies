using System;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Timberborn.CameraSystem;

public class ShadowDistanceUpdater : ILoadableSingleton, IUnloadableSingleton, ILateUpdatableSingleton
{
	public static readonly int MaxDistance = 150;

	private readonly CameraService _cameraService;

	private UniversalRenderPipelineAsset _originalPipelineAsset;

	private float _originalShadowDistance;

	public ShadowDistanceUpdater(CameraService cameraService)
	{
		_cameraService = cameraService;
	}

	public void Load()
	{
		_originalPipelineAsset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
		_originalShadowDistance = _originalPipelineAsset.shadowDistance;
		UpdateShadowDistance();
	}

	public void Unload()
	{
		UniversalRenderPipelineAsset universalRenderPipelineAsset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
		if (_originalPipelineAsset == universalRenderPipelineAsset)
		{
			universalRenderPipelineAsset.shadowDistance = _originalShadowDistance;
			QualitySettings.shadowDistance = _originalShadowDistance;
		}
	}

	public void LateUpdateSingleton()
	{
		UpdateShadowDistance();
	}

	public void SetShadowDistance(float shadowDistance)
	{
		QualitySettings.shadowDistance = shadowDistance;
		((UniversalRenderPipelineAsset)QualitySettings.renderPipeline).shadowDistance = shadowDistance;
	}

	public float GetShadowDistance()
	{
		return QualitySettings.shadowDistance;
	}

	private void UpdateShadowDistance()
	{
		float a = DistanceAtNormalizedScreenPoint(new Vector2(0f, 0f));
		float b = DistanceAtNormalizedScreenPoint(new Vector2(0f, 1f));
		float a2 = DistanceAtNormalizedScreenPoint(new Vector2(1f, 0f));
		float num = Mathf.Clamp(Mathf.Max(b: Mathf.Max(a2, DistanceAtNormalizedScreenPoint(new Vector2(1f, 1f))), a: Mathf.Max(a, b)), 0f, MaxDistance);
		if (Math.Abs(num - GetShadowDistance()) > 0.1f)
		{
			SetShadowDistance(num);
		}
	}

	private float DistanceAtNormalizedScreenPoint(Vector2 point)
	{
		Ray ray = NormalizedScreenPointToRay(point);
		if (new Plane(Vector3.up, 0f).Raycast(ray, out var enter))
		{
			return enter;
		}
		return float.MaxValue;
	}

	private Ray NormalizedScreenPointToRay(Vector2 point)
	{
		Vector2 vector = new Vector2(Screen.width, Screen.height);
		return _cameraService.ScreenPointToRayInWorldSpace(vector * point);
	}
}

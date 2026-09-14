using Timberborn.CameraSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.ThumbnailCapturing;

public class ThumbnailCamera : ILoadableSingleton, IPostLoadableSingleton
{
	private readonly CameraFactory _cameraFactory;

	private readonly CameraService _mainCamera;

	private readonly IThumbnailRenderTextureProvider _thumbnailRenderTextureProvider;

	private Camera _thumbnailCamera;

	public Transform Transform => _thumbnailCamera.transform;

	public ThumbnailCamera(CameraFactory cameraFactory, CameraService mainCamera, IThumbnailRenderTextureProvider thumbnailRenderTextureProvider)
	{
		_cameraFactory = cameraFactory;
		_mainCamera = mainCamera;
		_thumbnailRenderTextureProvider = thumbnailRenderTextureProvider;
	}

	public void Load()
	{
		_thumbnailCamera = _cameraFactory.Create("ThumbnailCamera");
		_thumbnailCamera.enabled = false;
	}

	public void PostLoad()
	{
		_thumbnailCamera.targetTexture = _thumbnailRenderTextureProvider.RenderTexture;
	}

	public void MoveToMainCameraPosition()
	{
		Transform transform = _mainCamera.Transform;
		SetPositionAndRotation(transform.position, transform.rotation);
	}

	public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
	{
		_thumbnailCamera.transform.SetPositionAndRotation(position, rotation);
	}

	public void Render()
	{
		_thumbnailCamera.Render();
	}
}

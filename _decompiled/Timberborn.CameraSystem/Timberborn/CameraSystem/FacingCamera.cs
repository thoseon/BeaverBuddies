using Timberborn.BaseComponentSystem;
using Timberborn.MortalComponents;
using UnityEngine;

namespace Timberborn.CameraSystem;

public class FacingCamera : BaseComponent, IAwakableComponent, ILateUpdatableComponent, IDeadNeededComponent
{
	private readonly CameraService _cameraService;

	private Transform _transform;

	public FacingCamera(CameraService cameraService)
	{
		_cameraService = cameraService;
	}

	public void Awake()
	{
		DisableComponent();
	}

	public void LateUpdate()
	{
		SetRotation();
	}

	public void Enable(Transform transformToRotate)
	{
		_transform = transformToRotate;
		SetRotation();
		EnableComponent();
	}

	public void Disable()
	{
		_transform = null;
		DisableComponent();
	}

	private void SetRotation()
	{
		_transform.rotation = _cameraService.FacingCamera;
	}
}

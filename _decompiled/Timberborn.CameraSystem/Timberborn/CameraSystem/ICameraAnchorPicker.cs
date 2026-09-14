using UnityEngine;

namespace Timberborn.CameraSystem;

public interface ICameraAnchorPicker
{
	Vector3? PickAnchorPoint(Ray ray);
}

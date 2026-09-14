using Timberborn.Common;
using UnityEngine;

namespace Timberborn.WaterSourceSystem;

internal class RegulatorTransform
{
	private static readonly float AnimationSpeed = 0.25f;

	private readonly Transform _transform;

	private readonly Vector3 _regulatingPosition;

	private readonly Quaternion _regulatingRotation;

	private readonly Vector3 _nonRegulatingPosition;

	private readonly Quaternion _nonRegulatingRotation;

	private float _animationProgress;

	private RegulatorTransform(Transform transform, Vector3 regulatingPosition, Quaternion regulatingRotation, Vector3 nonRegulatingPosition, Quaternion nonRegulatingRotation)
	{
		_transform = transform;
		_regulatingPosition = regulatingPosition;
		_regulatingRotation = regulatingRotation;
		_nonRegulatingPosition = nonRegulatingPosition;
		_nonRegulatingRotation = nonRegulatingRotation;
	}

	public static RegulatorTransform Create(GameObject parent, RegulatorTransformSpec spec, bool isRegulating)
	{
		Transform transform = parent.FindChildTransform(spec.TransformName);
		Vector3 localPosition = transform.localPosition;
		Quaternion localRotation = transform.localRotation;
		Vector3 nonRegulatingPosition = localPosition + spec.TargetOffset;
		Quaternion nonRegulatingRotation = Quaternion.Euler(spec.TargetRotation);
		RegulatorTransform regulatorTransform = new RegulatorTransform(transform, localPosition, localRotation, nonRegulatingPosition, nonRegulatingRotation);
		regulatorTransform.UpdateInstantly(isRegulating);
		return regulatorTransform;
	}

	public void UpdateSmoothly(bool isRegulating)
	{
		_animationProgress = Mathf.MoveTowards(_animationProgress, GetAnimationProgress(isRegulating), AnimationSpeed * Time.deltaTime);
		var (localPosition, localRotation) = GetTargetPositionAndRotation();
		_transform.SetLocalPositionAndRotation(localPosition, localRotation);
	}

	public void UpdateInstantly(bool isRegulating)
	{
		_animationProgress = GetAnimationProgress(isRegulating);
		var (localPosition, localRotation) = GetTargetPositionAndRotation();
		_transform.SetLocalPositionAndRotation(localPosition, localRotation);
	}

	private static float GetAnimationProgress(bool isRegulating)
	{
		return isRegulating ? 1 : 0;
	}

	private (Vector3, Quaternion) GetTargetPositionAndRotation()
	{
		return (Vector3.Lerp(_regulatingPosition, _nonRegulatingPosition, _animationProgress), Quaternion.Lerp(_regulatingRotation, _nonRegulatingRotation, _animationProgress));
	}
}

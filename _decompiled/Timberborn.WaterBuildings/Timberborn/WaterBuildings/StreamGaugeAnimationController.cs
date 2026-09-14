using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.WaterBuildings;

public class StreamGaugeAnimationController : BaseComponent, IAwakableComponent
{
	private StreamGaugeAnimationControllerSpec _streamGaugeAnimationControllerSpec;

	private Transform _marker;

	public void Awake()
	{
		_streamGaugeAnimationControllerSpec = GetComponent<StreamGaugeAnimationControllerSpec>();
		_marker = base.GameObject.FindChildTransform(_streamGaugeAnimationControllerSpec.MarkerName);
	}

	public void SetHeight(float newHeight)
	{
		Vector3 localPosition = _marker.localPosition;
		float y = Mathf.Min(newHeight, _streamGaugeAnimationControllerSpec.MaxHeight);
		localPosition = new Vector3(localPosition.x, y, localPosition.z);
		_marker.localPosition = localPosition;
	}
}

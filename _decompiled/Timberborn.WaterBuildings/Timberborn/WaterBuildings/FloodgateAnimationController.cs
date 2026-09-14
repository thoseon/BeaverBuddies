using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.WaterBuildings;

internal class FloodgateAnimationController : BaseComponent, IAwakableComponent, IUpdatableComponent
{
	private float _targetHeight;

	public Transform Gate { get; private set; }

	public void Awake()
	{
		string gateName = GetComponent<FloodgateAnimationControllerSpec>().GateName;
		Gate = base.GameObject.FindChildTransform(gateName);
		DisableComponent();
	}

	public void Update()
	{
		float height = GetHeight();
		SetHeight(height);
		if (Mathf.Abs(height - _targetHeight) < 0.001f)
		{
			DisableComponent();
		}
	}

	public void MoveGateInstantly(float height)
	{
		SetHeight(height);
		DisableComponent();
	}

	public void MoveGateSmoothly(float height)
	{
		_targetHeight = height;
		EnableComponent();
	}

	private float GetHeight()
	{
		return Mathf.MoveTowards(Gate.transform.localPosition.y, _targetHeight, Time.deltaTime * 3f);
	}

	private void SetHeight(float height)
	{
		Gate.transform.localPosition = new Vector3(0f, height, 0f);
	}
}

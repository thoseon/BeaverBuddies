using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.Terraforming;

internal class DrillHeadVisualizer : BaseComponent, IAwakableComponent, IUpdatableComponent, IInitializableEntity, IFinishedStateListener
{
	private BlockObject _blockObject;

	private Drill _drill;

	private DrillHeadVisualizerSpec _drillHeadVisualizerSpec;

	private Transform _headTransform;

	private bool _startedFinished;

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_drill = GetComponent<Drill>();
		_drillHeadVisualizerSpec = GetComponent<DrillHeadVisualizerSpec>();
		_headTransform = base.GameObject.FindChildTransform(_drillHeadVisualizerSpec.HeadTransformName);
		DisableComponent();
	}

	public void InitializeEntity()
	{
		GetComponent<DrillScrewRotator>().Add(_headTransform);
		if (_blockObject.IsFinished)
		{
			_startedFinished = true;
		}
	}

	public void Update()
	{
		UpdateHeadPosition();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		if (_startedFinished)
		{
			_headTransform.position = GetTargetPosition();
		}
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	private void UpdateHeadPosition()
	{
		Vector3 targetPosition = GetTargetPosition();
		Vector3 position = _headTransform.position;
		if (Math.Abs(targetPosition.y - position.y) > 0.0001f)
		{
			_headTransform.position = Vector3.MoveTowards(position, targetPosition, Time.deltaTime);
		}
	}

	private Vector3 GetTargetPosition()
	{
		float y = CoordinateSystem.GridToWorldCentered(new Vector3Int(0, 0, _drill.DrillingLevel)).y;
		Vector3 position = _headTransform.position;
		float headOffset = _drillHeadVisualizerSpec.HeadOffset;
		return new Vector3(position.x, y + headOffset, position.z);
	}
}

using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.MechanicalSystem;
using Timberborn.TickSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.PowerGeneration;

public class WaterPoweredGenerator : TickableComponent, IAwakableComponent, IFinishedPostLoadStateListener, IPostPlacementChangeListener
{
	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private BlockObject _blockObject;

	private MechanicalNode _mechanicalNode;

	private WaterPoweredGeneratorSpec _waterPoweredGeneratorSpec;

	private Vector2 _expectedWaterDirection;

	private readonly List<Vector3Int> _groundedCoordinates = new List<Vector3Int>();

	public float GeneratorRotation { get; private set; }

	public event EventHandler RotationUpdated;

	public WaterPoweredGenerator(IThreadSafeWaterMap threadSafeWaterMap)
	{
		_threadSafeWaterMap = threadSafeWaterMap;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_mechanicalNode = GetComponent<MechanicalNode>();
		_waterPoweredGeneratorSpec = GetComponent<WaterPoweredGeneratorSpec>();
		DisableComponent();
	}

	public void OnEnterFinishedPostLoadState()
	{
		ValidateWaterDirection();
		CalculateCoordinates();
		UpdateGenerator();
		EnableComponent();
	}

	public void OnExitFinishedPostLoadState()
	{
		DisableComponent();
	}

	public void OnPostPlacementChanged()
	{
		CalculateCoordinates();
	}

	public override void Tick()
	{
		UpdateGenerator();
	}

	public float CalculateGeneratedRotation()
	{
		float num = 0f;
		for (int i = 0; i < _groundedCoordinates.Count; i++)
		{
			Vector3Int coordinates = _groundedCoordinates[i];
			float num2 = CalculateGeneratedRotation(coordinates);
			if (Mathf.Abs(num2) > _waterPoweredGeneratorSpec.MinRequiredOutflow)
			{
				num += num2;
			}
		}
		return num / (float)_groundedCoordinates.Count;
	}

	private void CalculateCoordinates()
	{
		_expectedWaterDirection = _blockObject.Orientation.Transform(_waterPoweredGeneratorSpec.ExpectedWaterDirection);
		_groundedCoordinates.Clear();
		foreach (Vector2Int block in _waterPoweredGeneratorSpec.Blocks)
		{
			_groundedCoordinates.Add(_blockObject.TransformCoordinates(block.XYZ()));
		}
	}

	private void UpdateGenerator()
	{
		GeneratorRotation = CalculateGeneratedRotation();
		_mechanicalNode.SetOutputMultiplier(Math.Abs(GeneratorRotation));
		RotationUpdated?.Invoke(this, EventArgs.Empty);
	}

	private float CalculateGeneratedRotation(Vector3Int coordinates)
	{
		if (_threadSafeWaterMap.CellIsUnderwater(coordinates))
		{
			Vector2 vector = _threadSafeWaterMap.WaterFlowDirection(coordinates);
			if (_expectedWaterDirection.x != 0f)
			{
				return vector.x * (float)Math.Sign(_expectedWaterDirection.x);
			}
			if (_expectedWaterDirection.y != 0f)
			{
				return vector.y * (float)Math.Sign(_expectedWaterDirection.y);
			}
		}
		return 0f;
	}

	private void ValidateWaterDirection()
	{
		float x = _waterPoweredGeneratorSpec.ExpectedWaterDirection.x;
		float y = _waterPoweredGeneratorSpec.ExpectedWaterDirection.y;
		if ((x != 0f && y != 0f) || (x == 0f && y == 0f))
		{
			throw new InvalidOperationException(base.Name + " should have set either x or y of ExpectedWaterDirection");
		}
	}
}

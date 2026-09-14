using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Workshops;
using UnityEngine;

namespace Timberborn.Terraforming;

public class DrillScrewRotator : BaseComponent, IAwakableComponent, IUpdatableComponent
{
	private BlockObject _blockObject;

	private Workshop _workshop;

	private Drill _drill;

	private DrillScrewRotatorSpec _drillScrewRotatorSpec;

	private readonly List<Transform> _screwTransforms = new List<Transform>();

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_workshop = GetComponent<Workshop>();
		_drill = GetComponent<Drill>();
		_drillScrewRotatorSpec = GetComponent<DrillScrewRotatorSpec>();
	}

	public void Update()
	{
		float angle = CalculateRotationSpeed() * Time.deltaTime;
		foreach (Transform screwTransform in _screwTransforms)
		{
			screwTransform.Rotate(Vector3.up, angle);
		}
	}

	public void Add(Transform screwTransform)
	{
		_screwTransforms.Add(screwTransform);
	}

	private float CalculateRotationSpeed()
	{
		bool num = _drill.Enabled && _workshop.CurrentlyWorking;
		float minimumRotationSpeed = _drillScrewRotatorSpec.MinimumRotationSpeed;
		float rotationSpeedPerWorker = _drillScrewRotatorSpec.RotationSpeedPerWorker;
		return (num ? (minimumRotationSpeed + (float)_workshop.NumberOfWorkersWorking * rotationSpeedPerWorker) : 0f) * (float)(_blockObject.FlipMode.IsUnflipped ? 1 : (-1));
	}
}

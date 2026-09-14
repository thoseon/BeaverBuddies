using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;

namespace Timberborn.WaterSourceSystem;

internal class WaterSourceRegulatorAnimationController : BaseComponent, IAwakableComponent, IInitializableEntity, IUpdatableComponent
{
	private BlockObject _blockObject;

	private WaterSourceRegulator _waterSourceRegulator;

	private WaterSourceRegulatorAnimationControllerSpec _waterSourceRegulatorAnimationControllerSpec;

	private readonly List<RegulatorTransform> _regulatorTransforms = new List<RegulatorTransform>();

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_waterSourceRegulator = GetComponent<WaterSourceRegulator>();
		_waterSourceRegulatorAnimationControllerSpec = GetComponent<WaterSourceRegulatorAnimationControllerSpec>();
	}

	public void InitializeEntity()
	{
		foreach (RegulatorTransformSpec regulatorTransform in _waterSourceRegulatorAnimationControllerSpec.RegulatorTransforms)
		{
			_regulatorTransforms.Add(RegulatorTransform.Create(base.GameObject, regulatorTransform, _waterSourceRegulator.IsOpen));
		}
	}

	public void Update()
	{
		foreach (RegulatorTransform regulatorTransform in _regulatorTransforms)
		{
			if (_blockObject.IsFinished)
			{
				regulatorTransform.UpdateSmoothly(_waterSourceRegulator.IsOpen);
			}
			else
			{
				regulatorTransform.UpdateInstantly(_waterSourceRegulator.IsOpen);
			}
		}
	}
}

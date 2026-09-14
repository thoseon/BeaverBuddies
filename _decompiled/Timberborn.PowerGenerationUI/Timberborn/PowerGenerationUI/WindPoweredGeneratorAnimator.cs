using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.MechanicalSystem;
using Timberborn.TickSystem;
using Timberborn.WindSystem;

namespace Timberborn.PowerGenerationUI;

internal class WindPoweredGeneratorAnimator : TickableComponent, IAwakableComponent, IFinishedStateListener
{
	private WindRotationAnimator _windRotationAnimator;

	private MechanicalNode _mechanicalNode;

	public void Awake()
	{
		_windRotationAnimator = GetComponent<WindRotationAnimator>();
		_mechanicalNode = GetComponent<MechanicalNode>();
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public override void Tick()
	{
		if (_mechanicalNode.OutputMultiplier > 0f && _mechanicalNode.Active)
		{
			_windRotationAnimator.UnsuspendAnimation();
		}
		else
		{
			_windRotationAnimator.SuspendAnimation();
		}
	}
}

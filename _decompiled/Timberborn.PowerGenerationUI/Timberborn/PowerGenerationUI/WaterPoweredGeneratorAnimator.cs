using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.MechanicalSystem;
using Timberborn.PowerGeneration;
using Timberborn.SingletonSystem;
using Timberborn.TimbermeshAnimations;
using Timberborn.TimeSystem;

namespace Timberborn.PowerGenerationUI;

internal class WaterPoweredGeneratorAnimator : BaseComponent, IFinishedStateListener, IAwakableComponent
{
	private readonly WaterPoweredGeneratorSpeedCalculator _waterPoweredGeneratorSpeedCalculator;

	private readonly EventBus _eventBus;

	private WaterPoweredGenerator _waterPoweredGenerator;

	private MechanicalNode _mechanicalNode;

	private IAnimator _animator;

	public WaterPoweredGeneratorAnimator(WaterPoweredGeneratorSpeedCalculator waterPoweredGeneratorSpeedCalculator, EventBus eventBus)
	{
		_waterPoweredGeneratorSpeedCalculator = waterPoweredGeneratorSpeedCalculator;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_waterPoweredGenerator = GetComponent<WaterPoweredGenerator>();
		_mechanicalNode = GetComponent<MechanicalNode>();
		_animator = GetComponentInChildren<IAnimator>(includeInactive: true);
		_waterPoweredGenerator.RotationUpdated += delegate
		{
			UpdateAnimation();
		};
	}

	public void OnEnterFinishedState()
	{
		_eventBus.Register(this);
	}

	public void OnExitFinishedState()
	{
		_eventBus.Unregister(this);
	}

	[OnEvent]
	public void OnCurrentSpeedChanged(CurrentSpeedChangedEvent currentSpeedChangedEvent)
	{
		UpdateAnimation();
	}

	private void UpdateAnimation()
	{
		if (_mechanicalNode.Active)
		{
			float generatorRotation = _waterPoweredGenerator.GeneratorRotation;
			_animator.Enabled = true;
			_animator.Speed = _waterPoweredGeneratorSpeedCalculator.CalculateSpeed(generatorRotation);
		}
		else
		{
			_animator.Enabled = false;
			_animator.Speed = 0f;
		}
	}
}

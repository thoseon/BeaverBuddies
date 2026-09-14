using Timberborn.BaseComponentSystem;
using Timberborn.MechanicalSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.PowerGeneration;

internal class RotationMechanicalNodeUpdater : BaseComponent, IAwakableComponent
{
	private readonly EventBus _eventBus;

	private MechanicalNode _mechanicalNode;

	private WaterPoweredGenerator _waterGenerator;

	private bool _isReversed;

	public RotationMechanicalNodeUpdater(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_mechanicalNode = GetComponent<MechanicalNode>();
		_waterGenerator = GetComponent<WaterPoweredGenerator>();
		_waterGenerator.RotationUpdated += delegate
		{
			UpdateMechanicalNode();
		};
	}

	private void UpdateMechanicalNode()
	{
		bool flag = _waterGenerator.GeneratorRotation < 0f;
		if (_mechanicalNode.OutputMultiplier > 0f && flag != _isReversed)
		{
			_mechanicalNode.ReverseAllTransputs();
			_isReversed = flag;
			if (_mechanicalNode.Graph != null)
			{
				_eventBus.Post(new MechanicalGraphGeneratorUpdatedEvent(_mechanicalNode.Graph));
			}
		}
	}
}

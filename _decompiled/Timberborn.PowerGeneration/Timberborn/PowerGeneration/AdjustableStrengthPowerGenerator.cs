using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.MechanicalSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.PowerGeneration;

public class AdjustableStrengthPowerGenerator : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private readonly EventBus _eventBus;

	private MechanicalNode _mechanicalNode;

	public int MaxValue { get; private set; }

	public float GeneratorStrength
	{
		get
		{
			return _mechanicalNode.OutputMultiplier;
		}
		set
		{
			_mechanicalNode.SetOutputMultiplier(value);
		}
	}

	public AdjustableStrengthPowerGenerator(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Awake()
	{
		MaxValue = GetComponent<MechanicalNodeSpec>().PowerOutput;
		_mechanicalNode = GetComponent<MechanicalNode>();
	}

	public void InitializeEntity()
	{
		GeneratorStrength = 0.5f;
	}

	public void FlipRotation()
	{
		_mechanicalNode.ReverseAllTransputs();
		if (_mechanicalNode.Graph != null)
		{
			_eventBus.Post(new MechanicalGraphGeneratorUpdatedEvent(_mechanicalNode.Graph));
		}
	}
}

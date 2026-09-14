using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.MechanicalSystem;
using Timberborn.TickSystem;
using Timberborn.WindSystem;

namespace Timberborn.PowerGeneration;

public class WindPoweredGenerator : TickableComponent, IAwakableComponent, IInitializableEntity
{
	private readonly WindService _windService;

	private WindPoweredGeneratorSpec _windPoweredGeneratorSpec;

	private MechanicalNode _mechanicalNode;

	public WindPoweredGenerator(WindService windService)
	{
		_windService = windService;
	}

	public void Awake()
	{
		_windPoweredGeneratorSpec = GetComponent<WindPoweredGeneratorSpec>();
		_mechanicalNode = GetComponent<MechanicalNode>();
	}

	public void InitializeEntity()
	{
		UpdateOutput();
	}

	public override void Tick()
	{
		UpdateOutput();
	}

	private void UpdateOutput()
	{
		_mechanicalNode.SetOutputMultiplier((_windService.WindStrength > _windPoweredGeneratorSpec.MinRequiredWindStrength) ? _windService.WindStrength : 0f);
	}
}

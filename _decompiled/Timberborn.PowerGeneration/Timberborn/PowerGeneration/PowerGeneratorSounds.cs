using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.MechanicalSystem;
using Timberborn.TickSystem;

namespace Timberborn.PowerGeneration;

internal class PowerGeneratorSounds : TickableComponent, IAwakableComponent, IFinishedStateListener
{
	private BuildingSounds _buildingSounds;

	private MechanicalNode _mechanicalNode;

	public void Awake()
	{
		_buildingSounds = GetComponent<BuildingSounds>();
		_mechanicalNode = GetComponent<MechanicalNode>();
		DisableComponent();
	}

	public override void Tick()
	{
		UpdateSound();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	private void UpdateSound()
	{
		_buildingSounds.ToggleSound(_mechanicalNode.ActiveAndPowered && _mechanicalNode.OutputMultiplier > 0f);
	}
}

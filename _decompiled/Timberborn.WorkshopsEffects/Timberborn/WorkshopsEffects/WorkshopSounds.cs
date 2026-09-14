using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.TickSystem;
using Timberborn.Workshops;

namespace Timberborn.WorkshopsEffects;

internal class WorkshopSounds : TickableComponent, IAwakableComponent, IFinishedStateListener
{
	private BuildingSounds _buildingSounds;

	private Workshop _workshop;

	public void Awake()
	{
		_buildingSounds = GetComponent<BuildingSounds>();
		_workshop = GetComponent<Workshop>();
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
		_buildingSounds.ToggleSound(_workshop.CurrentlyWorking);
	}
}

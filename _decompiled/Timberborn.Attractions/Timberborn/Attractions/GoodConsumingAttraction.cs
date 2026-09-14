using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EnterableSystem;
using Timberborn.GoodConsumingBuildingSystem;

namespace Timberborn.Attractions;

internal class GoodConsumingAttraction : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private Enterable _enterable;

	private GoodConsumingToggle _goodConsumingToggle;

	public void Awake()
	{
		_goodConsumingToggle = GetComponent<GoodConsumingBuilding>().GetGoodConsumingToggle();
		_enterable = GetComponent<Enterable>();
		_enterable.EntererAdded += OnEntererAdded;
		_enterable.EntererRemoved += OnEntererRemoved;
	}

	public void OnEnterFinishedState()
	{
		UpdateState();
	}

	public void OnExitFinishedState()
	{
	}

	private void OnEntererAdded(object sender, EntererAddedEventArgs e)
	{
		UpdateState();
	}

	private void OnEntererRemoved(object sender, EntererRemovedEventArgs e)
	{
		UpdateState();
	}

	private void UpdateState()
	{
		if (_enterable.NumberOfEnterersInside > 0)
		{
			_goodConsumingToggle.ResumeConsumption();
		}
		else
		{
			_goodConsumingToggle.PauseConsumption();
		}
	}
}

using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EnterableSystem;

namespace Timberborn.NeedSuspending;

public class EntererNeedSuspendingBuilding : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private Enterable _enterable;

	private EntererNeedSuspendingBuildingSpec _entererNeedSuspendingBuildingSpec;

	public void Awake()
	{
		_enterable = GetComponent<Enterable>();
		_entererNeedSuspendingBuildingSpec = GetComponent<EntererNeedSuspendingBuildingSpec>();
	}

	public void OnEnterFinishedState()
	{
		_enterable.EntererAdded += OnEntererAdded;
		_enterable.EntererRemoved += OnEntererRemoved;
	}

	public void OnExitFinishedState()
	{
		_enterable.EntererAdded -= OnEntererAdded;
		_enterable.EntererRemoved -= OnEntererRemoved;
	}

	private void OnEntererAdded(object sender, EntererAddedEventArgs e)
	{
		_entererNeedSuspendingBuildingSpec.NeedSuspender.SuspendNeeds(e.Enterer);
	}

	private void OnEntererRemoved(object sender, EntererRemovedEventArgs e)
	{
		_entererNeedSuspendingBuildingSpec.NeedSuspender.ResumeNeeds(e.Enterer);
	}
}

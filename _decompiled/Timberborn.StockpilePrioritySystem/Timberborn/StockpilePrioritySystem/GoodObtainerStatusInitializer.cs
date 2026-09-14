using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Hauling;

namespace Timberborn.StockpilePrioritySystem;

internal class GoodObtainerStatusInitializer : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private GoodObtainer _goodObtainer;

	private NoHaulingPostStatus _noHaulingPostStatus;

	public void Awake()
	{
		_goodObtainer = GetComponent<GoodObtainer>();
		_noHaulingPostStatus = GetComponent<NoHaulingPostStatus>();
	}

	public void OnEnterFinishedState()
	{
		_noHaulingPostStatus.Initialize(() => _goodObtainer.IsObtaining);
		_goodObtainer.GoodObtainingChanged += UpdateStatus;
	}

	public void OnExitFinishedState()
	{
		_goodObtainer.GoodObtainingChanged -= UpdateStatus;
		_noHaulingPostStatus.Disable();
	}

	private void UpdateStatus(object sender, EventArgs e)
	{
		_noHaulingPostStatus.UpdateStatus();
	}
}

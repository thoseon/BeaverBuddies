using System;
using Timberborn.BaseComponentSystem;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;

namespace Timberborn.WorkSystem;

public class WorkRefuser : BaseComponent, IAwakableComponent
{
	private NeedManager _needManager;

	private Worker _worker;

	public bool RefusesWork { get; private set; }

	public event EventHandler RefusesWorkChanged;

	public void Awake()
	{
		_needManager = GetComponent<NeedManager>();
		_needManager.NeedChangedCriticalState += OnNeedChangedCriticalState;
		_worker = GetComponent<Worker>();
	}

	private void OnNeedChangedCriticalState(object sender, NeedChangedCriticalStateEventArgs e)
	{
		UpdateRefuseWork();
	}

	private void UpdateRefuseWork()
	{
		bool flag = ShouldRefuseWork();
		if (RefusesWork != flag)
		{
			RefusesWork = flag;
			if (RefusesWork)
			{
				_worker.Unemploy();
			}
			RefusesWorkChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private bool ShouldRefuseWork()
	{
		foreach (NeedSpec needSpec in _needManager.NeedSpecs)
		{
			if (_needManager.NeedIsInCriticalState(needSpec.Id) && needSpec.HasSpec<NeedPreventingWorkSpec>())
			{
				return true;
			}
		}
		return false;
	}
}

using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using Timberborn.StatusSystem;

namespace Timberborn.NeedBehaviorSystem;

public class CriticalNeedStateStatusRegistrar : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private NeedManager _needManager;

	private readonly List<StatusToggle> _statusToggles = new List<StatusToggle>();

	public void Awake()
	{
		_needManager = GetComponent<NeedManager>();
		InitializeStatusToggles();
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatuses(_statusToggles.AsReadOnlyEnumerable());
	}

	private void InitializeStatusToggles()
	{
		foreach (NeedSpec needSpec in _needManager.NeedSpecs)
		{
			CriticalNeedSpec spec = needSpec.GetSpec<CriticalNeedSpec>();
			if ((object)spec != null)
			{
				if (spec.CriticalNeedType == CriticalNeedType.State)
				{
					StatusToggle statusToggle = StatusToggle.CreateNormalStatusWithFloatingIcon(spec.SpriteName, spec.Description.Value);
					InitializeStatusToggle(needSpec, statusToggle);
				}
				if (spec.CriticalNeedType == CriticalNeedType.Alert)
				{
					StatusToggle statusToggle2 = StatusToggle.CreateNormalStatusWithAlert(spec.SpriteName, spec.Description.Value, spec.DescriptionShort.Value);
					InitializeStatusToggle(needSpec, statusToggle2);
				}
				if (spec.CriticalNeedType == CriticalNeedType.StateWithAlert)
				{
					StatusToggle statusToggle3 = StatusToggle.CreateNormalStatusWithAlertAndFloatingIcon(spec.SpriteName, spec.Description.Value, spec.DescriptionShort.Value);
					InitializeStatusToggle(needSpec, statusToggle3);
				}
			}
		}
	}

	private void InitializeStatusToggle(NeedSpec needSpec, StatusToggle statusToggle)
	{
		_needManager.NeedChangedCriticalState += OnNeedChangedCriticalState;
		_statusToggles.Add(statusToggle);
		void OnNeedChangedCriticalState(object sender, NeedChangedCriticalStateEventArgs e)
		{
			if (e.NeedSpec == needSpec)
			{
				if (e.IsInCriticalState)
				{
					statusToggle.Activate();
				}
				else
				{
					statusToggle.Deactivate();
				}
			}
		}
	}
}

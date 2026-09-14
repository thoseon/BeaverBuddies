using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using Timberborn.StatusSystem;
using Timberborn.TickSystem;

namespace Timberborn.NeedBehaviorSystem;

public class CriticalNeedActionStatusRegistrar : TickableComponent, IAwakableComponent, IPostLoadableEntity
{
	private class NeedStatusToggle
	{
		public string NeedId { get; }

		public StatusToggle StatusToggle { get; }

		public NeedStatusToggle(string needId, StatusToggle statusToggle)
		{
			NeedId = needId;
			StatusToggle = statusToggle;
		}
	}

	private readonly List<NeedStatusToggle> _needStatusToggles = new List<NeedStatusToggle>();

	private NeedManager _needManager;

	private INeedBehaviorPicker _needBehaviorPicker;

	private CriticalNeederRootBehavior _criticalNeederRootBehavior;

	public void Awake()
	{
		_needManager = GetComponent<NeedManager>();
		_needBehaviorPicker = GetComponent<INeedBehaviorPicker>();
		_criticalNeederRootBehavior = GetComponent<CriticalNeederRootBehavior>();
		InitializeNeedStatusToggles();
	}

	public void PostLoadEntity()
	{
		UpdateNeedStatuses();
		GetComponent<StatusSubject>().RegisterStatuses(_needStatusToggles.Select((NeedStatusToggle needStatusToggle) => needStatusToggle.StatusToggle));
	}

	public override void Tick()
	{
		UpdateNeedStatuses();
	}

	private void InitializeNeedStatusToggles()
	{
		foreach (NeedSpec needSpec in _needManager.NeedSpecs)
		{
			CriticalNeedSpec spec = needSpec.GetSpec<CriticalNeedSpec>();
			if ((object)spec != null && spec.CriticalNeedType == CriticalNeedType.Action)
			{
				InitializeNeedStatusToggle(needSpec, spec);
			}
		}
	}

	private void InitializeNeedStatusToggle(NeedSpec needSpec, CriticalNeedSpec criticalNeedSpec)
	{
		StatusToggle statusToggle = StatusToggle.CreateNormalStatusWithFloatingIcon(criticalNeedSpec.SpriteName, criticalNeedSpec.Description.Value);
		NeedStatusToggle item = new NeedStatusToggle(needSpec.Id, statusToggle);
		_needStatusToggles.Add(item);
	}

	private void UpdateNeedStatuses()
	{
		for (int i = 0; i < _needStatusToggles.Count; i++)
		{
			UpdateNeedStatus(_needStatusToggles[i]);
		}
	}

	private void UpdateNeedStatus(NeedStatusToggle needStatusToggle)
	{
		StatusToggle statusToggle = needStatusToggle.StatusToggle;
		if (NeedIsBeingCriticallySatisfied(needStatusToggle.NeedId))
		{
			statusToggle.Activate();
		}
		else
		{
			statusToggle.Deactivate();
		}
	}

	private bool NeedIsBeingCriticallySatisfied(string needId)
	{
		if (_criticalNeederRootBehavior.NeedRunning)
		{
			return _needBehaviorPicker.NeedIsBeingCriticallySatisfied(needId);
		}
		return false;
	}
}

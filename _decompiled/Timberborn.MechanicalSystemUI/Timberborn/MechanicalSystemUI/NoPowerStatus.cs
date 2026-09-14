using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.MechanicalSystem;
using Timberborn.StatusSystem;
using Timberborn.TickSystem;

namespace Timberborn.MechanicalSystemUI;

internal class NoPowerStatus : TickableComponent, IAwakableComponent, IPostLoadableEntity
{
	private static readonly string NoPowerSpriteName = "NoPower";

	private static readonly string NoPowerLocKey = "Status.Mechanical.NoPower";

	private static readonly string NoPowerShortLocKey = "Status.Mechanical.NoPower.Short";

	private readonly ILoc _loc;

	private MechanicalNode _mechanicalNode;

	private StatusToggle _noPowerStatusToggle;

	public NoPowerStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_mechanicalNode = GetComponent<MechanicalNode>();
		_mechanicalNode.AddedToGraph += OnAddedToGraph;
		DisableComponent();
	}

	public void PostLoadEntity()
	{
		if (base.Enabled)
		{
			UpdateStatus();
		}
	}

	public override void Tick()
	{
		UpdateStatus();
	}

	private void OnAddedToGraph(object sender, EventArgs e)
	{
		if (_mechanicalNode.IsConsumer)
		{
			InitializeStatus();
			UpdateStatus();
			EnableComponent();
		}
	}

	private void InitializeStatus()
	{
		if (_noPowerStatusToggle == null)
		{
			if (HasComponent<NoPowerStatusAlertDisablerSpec>())
			{
				_noPowerStatusToggle = StatusToggle.CreateNormalStatus(NoPowerSpriteName, _loc.T(NoPowerLocKey));
			}
			else
			{
				_noPowerStatusToggle = StatusToggle.CreateNormalStatusWithAlertAndFloatingIcon(NoPowerSpriteName, _loc.T(NoPowerLocKey), _loc.T(NoPowerShortLocKey));
			}
			GetComponent<StatusSubject>().RegisterStatus(_noPowerStatusToggle);
		}
	}

	private void UpdateStatus()
	{
		if (_mechanicalNode.CanPotentiallyBePowered())
		{
			if (_mechanicalNode.Powered || !_mechanicalNode.IsConsuming)
			{
				_noPowerStatusToggle.Deactivate();
			}
			else
			{
				_noPowerStatusToggle.Activate();
			}
		}
		else
		{
			_noPowerStatusToggle.Activate();
		}
	}
}

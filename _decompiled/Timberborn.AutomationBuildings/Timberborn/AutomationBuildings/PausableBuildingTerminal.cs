using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.Buildings;
using Timberborn.Emptying;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.StatusSystem;

namespace Timberborn.AutomationBuildings;

public class PausableBuildingTerminal : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener, ITerminal
{
	private static readonly string PausedByAutomationLocKey = "Automation.PausedByAutomation";

	private readonly ILoc _loc;

	private Automatable _automatable;

	private BlockableObject _blockableObject;

	private PausableBuilding _pausableBuilding;

	private StatusToggle _statusToggle;

	private AutoEmptiableBlockerToggle _autoEmptiableBlockerToggle;

	public PausableBuildingTerminal(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_automatable = GetComponent<Automatable>();
		_blockableObject = GetComponent<BlockableObject>();
		_pausableBuilding = GetComponent<PausableBuilding>();
		_statusToggle = StatusToggle.CreatePriorityStatusWithFloatingIcon("PausedByAutomation", _loc.T(PausedByAutomationLocKey));
		AutoEmptiableBlocker component = GetComponent<AutoEmptiableBlocker>();
		if (component != null)
		{
			_autoEmptiableBlockerToggle = component.CreateToggle();
		}
		DisableComponent();
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_statusToggle);
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		UpdateBlockable();
		_pausableBuilding.PausedChanged += OnPausedChanged;
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		UpdateBlockable();
		_pausableBuilding.PausedChanged -= OnPausedChanged;
	}

	public void Evaluate()
	{
		UpdateBlockable();
	}

	private void OnPausedChanged(object sender, EventArgs e)
	{
		UpdateStatusToggle();
		UpdateEmptiable();
	}

	private void UpdateBlockable()
	{
		if (ShouldBlock())
		{
			_blockableObject.Block(this);
			UpdateStatusToggle();
		}
		else
		{
			_blockableObject.Unblock(this);
			UpdateStatusToggle();
		}
		UpdateEmptiable();
	}

	private void UpdateEmptiable()
	{
		if (ShouldBlock() && !_pausableBuilding.Paused)
		{
			_autoEmptiableBlockerToggle?.Block();
		}
		else
		{
			_autoEmptiableBlockerToggle?.Unblock();
		}
	}

	private void UpdateStatusToggle()
	{
		_statusToggle.Toggle(ShouldBlock() && !_pausableBuilding.Paused);
	}

	private bool ShouldBlock()
	{
		if (base.Enabled)
		{
			return _automatable.State == ConnectionState.Off;
		}
		return false;
	}
}

using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.AutomationBuildings;

public class Gate : BaseComponent, IAwakableComponent, IDeletableEntity, IPersistentEntity, IFinishedStateListener, IAutomatableNeeder, IDuplicable<Gate>, IDuplicable, ITerminal
{
	private static readonly ComponentKey ComponentKey = new ComponentKey("Gate");

	private static readonly PropertyKey<GateOpeningMode> OpeningModeKey = new PropertyKey<GateOpeningMode>("OpeningMode");

	private readonly GateUpdater _gateUpdater;

	private BlockObject _blockObject;

	private Automatable _automatable;

	private GateNavMeshBlocker _gateNavMeshBlocker;

	private GateOpeningMode _gateOpeningMode;

	public bool IsConflict { get; private set; }

	public bool OpenMode => _gateOpeningMode == GateOpeningMode.Open;

	public bool ClosedMode => _gateOpeningMode == GateOpeningMode.Closed;

	public bool AutomatedMode => _gateOpeningMode == GateOpeningMode.Automated;

	public bool NeedsAutomatable => AutomatedMode;

	public bool IsOpenByAutomation
	{
		get
		{
			if (AutomatedMode)
			{
				return _automatable.State != ConnectionState.Off;
			}
			return false;
		}
	}

	public event EventHandler StateChanged;

	internal Gate(GateUpdater gateUpdater)
	{
		_gateUpdater = gateUpdater;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_automatable = GetComponent<Automatable>();
		_gateNavMeshBlocker = GetComponent<GateNavMeshBlocker>();
	}

	public void DeleteEntity()
	{
		if (_gateNavMeshBlocker.NavMeshBlocked)
		{
			_gateNavMeshBlocker.Unblock();
		}
		_gateUpdater.Remove(this);
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(ComponentKey).Set(OpeningModeKey, _gateOpeningMode);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ComponentKey);
		_gateOpeningMode = component.Get(OpeningModeKey);
	}

	public void OnEnterFinishedState()
	{
		UpdateState();
	}

	public void OnExitFinishedState()
	{
	}

	public void DuplicateFrom(Gate source)
	{
		_gateOpeningMode = source._gateOpeningMode;
		UpdateState();
	}

	public void Evaluate()
	{
		if (_gateOpeningMode == GateOpeningMode.Automated)
		{
			UpdateState();
		}
	}

	public void Open()
	{
		SetOpeningMode(GateOpeningMode.Open);
	}

	public void Close()
	{
		SetOpeningMode(GateOpeningMode.Closed);
	}

	public void Automate()
	{
		SetOpeningMode(GateOpeningMode.Automated);
	}

	public void EnableConflict()
	{
		IsConflict = true;
		NotifyStateChanged();
	}

	public void DisableConflict()
	{
		IsConflict = false;
		NotifyStateChanged();
	}

	internal void BlockNavMesh()
	{
		_gateNavMeshBlocker.Block();
		NotifyStateChanged();
	}

	internal void UnblockNavMesh()
	{
		_gateNavMeshBlocker.Unblock();
		NotifyStateChanged();
	}

	private void SetOpeningMode(GateOpeningMode gateOpeningMode)
	{
		if (_gateOpeningMode != gateOpeningMode)
		{
			_gateOpeningMode = gateOpeningMode;
			UpdateState();
		}
	}

	private void UpdateState()
	{
		if (_blockObject.IsFinished)
		{
			if (_gateOpeningMode == GateOpeningMode.Open || IsOpenByAutomation)
			{
				_gateUpdater.ScheduleToOpen(this);
			}
			else
			{
				_gateUpdater.ScheduleToClose(this);
			}
		}
	}

	private void NotifyStateChanged()
	{
		StateChanged?.Invoke(this, EventArgs.Empty);
	}
}

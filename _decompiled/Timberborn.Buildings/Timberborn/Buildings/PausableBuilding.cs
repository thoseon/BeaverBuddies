using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.Persistence;
using Timberborn.StatusSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Buildings;

public class PausableBuilding : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener, IUnfinishedStateListener, IPersistentEntity, IDuplicable<PausableBuilding>, IDuplicable
{
	private static readonly string PausedLocKey = "Status.Buildings.Paused";

	private static readonly ComponentKey PausableBuildingKey = new ComponentKey("PausableBuilding");

	private static readonly PropertyKey<bool> PausedKey = new PropertyKey<bool>("Paused");

	private readonly ILoc _loc;

	private BlockableObject _blockableObject;

	private BlockObject _blockObject;

	private readonly List<IUnfinishedPausable> _unfinishedPausables = new List<IUnfinishedPausable>();

	private readonly List<IFinishedPausable> _finishedPausables = new List<IFinishedPausable>();

	private StatusToggle _pauseStatusToggle;

	public bool Paused { get; private set; }

	public bool IsDuplicable => IsPausable();

	public event EventHandler PausedChanged;

	public PausableBuilding(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		_blockObject = GetComponent<BlockObject>();
		GetComponents(_unfinishedPausables);
		GetComponents(_finishedPausables);
		_pauseStatusToggle = StatusToggle.CreatePriorityStatusWithFloatingIcon("Pause", _loc.T(PausedLocKey));
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_pauseStatusToggle);
	}

	public void OnEnterFinishedState()
	{
	}

	public void OnExitFinishedState()
	{
		Resume();
	}

	public void OnEnterUnfinishedState()
	{
	}

	public void OnExitUnfinishedState()
	{
		Resume();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (Paused)
		{
			entitySaver.GetComponent(PausableBuildingKey).Set(PausedKey, Paused);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(PausableBuildingKey, out var objectLoader) && objectLoader.Get(PausedKey))
		{
			Pause();
		}
	}

	public void DuplicateFrom(PausableBuilding source)
	{
		if (IsPausable() && _blockObject.IsFinished == source.GetComponent<BlockObject>().IsFinished)
		{
			if (source.Paused)
			{
				Pause();
			}
			else
			{
				Resume();
			}
		}
	}

	public bool IsPausable()
	{
		if (!_blockObject.IsUnfinished || _unfinishedPausables.IsEmpty())
		{
			if (_blockObject.IsFinished)
			{
				return !_finishedPausables.IsEmpty();
			}
			return false;
		}
		return true;
	}

	public void Pause()
	{
		if (!Paused)
		{
			Paused = true;
			_blockableObject.Block(this);
			_pauseStatusToggle.Activate();
			PausedChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public void Resume()
	{
		if (Paused)
		{
			Paused = false;
			_blockableObject.Unblock(this);
			_pauseStatusToggle.Deactivate();
			PausedChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}

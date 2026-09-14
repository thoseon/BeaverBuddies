using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.EnterableSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.RelationSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.WorkSystem;

public class Workplace : BaseComponent, IAwakableComponent, IPostLoadableEntity, IPersistentEntity, IDuplicable<Workplace>, IDuplicable, IFinishedStateListener, IRegisteredComponent, IFinishedPausable, IRelationOwner, IStatusHider
{
	private static readonly ComponentKey WorkplaceKey = new ComponentKey("Workplace");

	private static readonly PropertyKey<int> DesiredWorkersKey = new PropertyKey<int>("DesiredWorkers");

	private WorkplaceSpec _workplaceSpec;

	private BlockableObject _blockableObject;

	private WorkplaceWorkerType _workplaceWorkerType;

	private readonly List<Worker> _assignedWorkers = new List<Worker>();

	private readonly List<WorkplaceBehavior> _workplaceBehaviors = new List<WorkplaceBehavior>();

	public int DesiredWorkers { get; private set; }

	public ReadOnlyList<Worker> AssignedWorkers => _assignedWorkers.AsReadOnlyList();

	public ReadOnlyList<WorkplaceBehavior> WorkplaceBehaviors => _workplaceBehaviors.AsReadOnlyList();

	public bool Understaffed
	{
		get
		{
			if (_blockableObject.IsUnblocked)
			{
				return _assignedWorkers.Count < DesiredWorkers;
			}
			return false;
		}
	}

	public bool Overstaffed
	{
		get
		{
			if (_blockableObject.IsUnblocked)
			{
				return _assignedWorkers.Count > DesiredWorkers;
			}
			return true;
		}
	}

	public int NumberOfAssignedWorkers => _assignedWorkers.Count;

	public int MaxWorkers => _workplaceSpec.MaxWorkers;

	public event EventHandler<WorkerChangedEventArgs> WorkerAssigned;

	public event EventHandler<WorkerChangedEventArgs> WorkerUnassigned;

	public event EventHandler RelationsChanged;

	public event EventHandler DesiredWorkersChanged;

	public void Awake()
	{
		_workplaceSpec = GetComponent<WorkplaceSpec>();
		_blockableObject = GetComponent<BlockableObject>();
		_workplaceWorkerType = GetComponent<WorkplaceWorkerType>();
		DesiredWorkers = _workplaceSpec.DefaultWorkers;
		DisableComponent();
	}

	public void PostLoadEntity()
	{
		ValidateWorkplaceSpec();
		ValidateWorkplaceBehaviors();
		ValidateEnterable();
		_blockableObject.ObjectBlocked += OnObjectBlocked;
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		UnassignAllWorkers();
		DisableComponent();
	}

	public void AssignWorker(Worker worker)
	{
		if (!_assignedWorkers.Contains(worker))
		{
			if (!Understaffed)
			{
				throw new InvalidOperationException($"Can't assign {worker} to {base.Name}, this workplace isn't understaffed");
			}
			if (_workplaceWorkerType.WorkerType != worker.WorkerType)
			{
				string text = "WorkerType";
				throw new InvalidOperationException($"Can't assign {worker} with {text} {worker.WorkerType}" + " to " + base.Name + " with " + text + " " + _workplaceWorkerType.WorkerType);
			}
			_assignedWorkers.Add(worker);
			worker.EmployAt(this);
			WorkerAssigned?.Invoke(this, new WorkerChangedEventArgs(worker));
			RelationsChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public void IncreaseDesiredWorkers()
	{
		if (DesiredWorkers < _workplaceSpec.MaxWorkers)
		{
			SetDesiredWorkers(DesiredWorkers + 1);
		}
	}

	public void DecreaseDesiredWorkers()
	{
		if (DesiredWorkers > 1)
		{
			SetDesiredWorkers(DesiredWorkers - 1);
		}
	}

	public void UnassignWorker(Worker worker)
	{
		if (_assignedWorkers.Remove(worker))
		{
			worker.Unemploy();
			WorkerUnassigned?.Invoke(this, new WorkerChangedEventArgs(worker));
			RelationsChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public bool AnyWorkerHasJobRunning()
	{
		for (int i = 0; i < _assignedWorkers.Count; i++)
		{
			if (_assignedWorkers[i].JobRunning)
			{
				return true;
			}
		}
		return false;
	}

	public void UnassignAllWorkers()
	{
		while (!_assignedWorkers.IsEmpty())
		{
			UnassignWorker(_assignedWorkers.First());
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(WorkplaceKey).Set(DesiredWorkersKey, DesiredWorkers);
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(WorkplaceKey, out var objectLoader))
		{
			DesiredWorkers = Math.Min(objectLoader.Get(DesiredWorkersKey), _workplaceSpec.MaxWorkers);
		}
	}

	public void DuplicateFrom(Workplace source)
	{
		if (source._workplaceSpec.MaxWorkers == _workplaceSpec.MaxWorkers)
		{
			SetDesiredWorkers(source.DesiredWorkers);
		}
	}

	public IEnumerable<BaseComponent> GetRelations()
	{
		return _assignedWorkers.Select((Worker assignedWorker) => assignedWorker);
	}

	private void OnObjectBlocked(object sender, EventArgs e)
	{
		UnassignAllWorkers();
	}

	private void ValidateWorkplaceSpec()
	{
		if (_workplaceSpec.DisallowOtherWorkerTypes && _workplaceSpec.WorkerTypeUnlockCosts.Length > 0)
		{
			throw new InvalidDataException(base.Name + ". If DisallowOtherWorkerTypes is set, WorkerTypeUnlockCosts must be empty.");
		}
	}

	private void ValidateWorkplaceBehaviors()
	{
		GetComponents(_workplaceBehaviors);
		if (_workplaceBehaviors.IsEmpty())
		{
			throw new InvalidOperationException(base.Name + " doesn't have any required WorkplaceBehavior component");
		}
	}

	private void ValidateEnterable()
	{
		int maxWorkers = _workplaceSpec.MaxWorkers;
		EnterableSpec component = GetComponent<EnterableSpec>();
		if (component.LimitedCapacityFinished && component.CapacityFinished < maxWorkers)
		{
			throw new InvalidOperationException($"{base.Name} has insufficient capacity for {maxWorkers} workers");
		}
	}

	private void SetDesiredWorkers(int desiredWorkers)
	{
		DesiredWorkers = desiredWorkers;
		DesiredWorkersChanged?.Invoke(this, EventArgs.Empty);
		UnassignWorkerIfOverstaffed();
	}

	private void UnassignWorkerIfOverstaffed()
	{
		if (Overstaffed)
		{
			UnassignWorkerIfNonworking();
		}
	}

	private void UnassignWorkerIfNonworking()
	{
		Worker worker = _assignedWorkers.FirstOrDefault((Worker worker2) => !worker2.JobRunning);
		if (worker != null)
		{
			UnassignWorker(worker);
		}
	}
}

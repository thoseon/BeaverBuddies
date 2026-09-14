using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.BonusSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.Characters;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.RelationSystem;
using Timberborn.TickSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WorkSystem;

public class Worker : TickableComponent, IAwakableComponent, IPersistentEntity, IInitializableEntity, IDeletableEntity, IRegisteredComponent, IRelationOwner
{
	private static readonly string WorkingSpeedBonusId = "WorkingSpeed";

	private static readonly ComponentKey WorkerKey = new ComponentKey("Worker");

	private static readonly PropertyKey<Workplace> WorkplaceKey = new PropertyKey<Workplace>("Workplace");

	private readonly ReferenceSerializer _referenceSerializer;

	private CharacterAnimator _characterAnimator;

	private BonusManager _bonusManager;

	private BehaviorManager _behaviorManager;

	private WorkerSpec _workerSpec;

	private float _workingSpeedMultiplier = 1f;

	private bool _loaded;

	public Workplace Workplace { get; private set; }

	public string WorkerType => _workerSpec.WorkerType;

	public bool JobRunning => _behaviorManager.IsRunningBehavior<IJobBehavior>();

	public bool Employed
	{
		get
		{
			if (base.Enabled)
			{
				return Workplace;
			}
			return false;
		}
	}

	public float WorkingSpeedMultiplier
	{
		get
		{
			return _workingSpeedMultiplier;
		}
		private set
		{
			_workingSpeedMultiplier = value;
			_characterAnimator.SetFloat("WorkingSpeed", value);
		}
	}

	public event EventHandler<EventArgs> GotEmployed;

	public event EventHandler<EventArgs> GotUnemployed;

	public event EventHandler RelationsChanged;

	public Worker(ReferenceSerializer referenceSerializer)
	{
		_referenceSerializer = referenceSerializer;
	}

	public void Awake()
	{
		DisableComponent();
		_characterAnimator = GetComponent<CharacterAnimator>();
		_bonusManager = GetComponent<BonusManager>();
		_behaviorManager = GetComponent<BehaviorManager>();
		_workerSpec = GetComponent<WorkerSpec>();
		GetComponent<Character>().Died += delegate
		{
			Unemploy();
		};
	}

	public void InitializeEntity()
	{
		WorkingSpeedMultiplier = 1f;
		if (_loaded)
		{
			AssignToWorkplaceAfterLoad();
		}
		EnableComponent();
	}

	public override void Tick()
	{
		WorkingSpeedMultiplier = _bonusManager.Multiplier(WorkingSpeedBonusId);
	}

	public void DeleteEntity()
	{
		Unemploy();
	}

	public void EmployAt(Workplace workplace)
	{
		if (Workplace != workplace)
		{
			Unemploy();
			SetWorkplace(workplace);
		}
	}

	public void Unemploy()
	{
		if (Workplace != null)
		{
			Workplace workplace = Workplace;
			Workplace = null;
			workplace.UnassignWorker(this);
			GotUnemployed?.Invoke(this, EventArgs.Empty);
			RelationsChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		if ((bool)Workplace)
		{
			entitySaver.GetComponent(WorkerKey).Set(WorkplaceKey, Workplace, _referenceSerializer.Of<Workplace>());
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(WorkerKey, out var objectLoader) && objectLoader.GetObsoletable(WorkplaceKey, _referenceSerializer.Of<Workplace>(), out var value))
		{
			Workplace = value;
			_loaded = true;
		}
	}

	public IEnumerable<BaseComponent> GetRelations()
	{
		if (!Employed)
		{
			return Enumerable.Empty<BaseComponent>();
		}
		return Enumerables.One(Workplace);
	}

	private void SetWorkplace(Workplace workplace)
	{
		Workplace = workplace;
		workplace.AssignWorker(this);
		GotEmployed?.Invoke(this, EventArgs.Empty);
		RelationsChanged?.Invoke(this, EventArgs.Empty);
	}

	private void AssignToWorkplaceAfterLoad()
	{
		if (!Workplace)
		{
			return;
		}
		if (!Workplace.GetComponent<EntityComponent>().Deleted && Workplace.Understaffed)
		{
			WorkplaceWorkerType component = Workplace.GetComponent<WorkplaceWorkerType>();
			if (component != null && component.WorkerType == WorkerType)
			{
				SetWorkplace(Workplace);
				return;
			}
		}
		string firstName = GetComponent<Character>().FirstName;
		Debug.LogWarning("After loading " + firstName + " couldn't get employed at their old workplace, " + Workplace.Name);
		Workplace = null;
	}
}

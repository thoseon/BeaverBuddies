using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.ReservableSystem;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Demolishing;

public class Demolishable : BaseComponent, IAwakableComponent, IPersistentEntity, IDeletableEntity, IDuplicable<Demolishable>, IDuplicable, IInitializableEntity
{
	private static readonly ComponentKey DemolishableKey = new ComponentKey("Demolishable");

	private static readonly PropertyKey<bool> IsMarkedKey = new PropertyKey<bool>("IsMarked");

	private static readonly PropertyKey<float> DemolishTimeLeft = new PropertyKey<float>("DemolishTimeLeft");

	private readonly EventBus _eventBus;

	private readonly EntityService _entityService;

	private DemolishJob _demolishJob;

	private DemolishableSpec _demolishableSpec;

	private BlockObject _blockObject;

	private bool _markPostLoad;

	private float _demolishTimeLeft;

	public bool IsMarked { get; private set; }

	public Reservable Reservable { get; private set; }

	public float DemolishingProgress => 1f - _demolishTimeLeft / _demolishableSpec.DemolishTimeInHours;

	public bool ShowDemolishButtonInEntityPanel => _demolishableSpec.ShowDemolishButtonInEntityPanel;

	public event EventHandler Marked;

	public event EventHandler Unmarked;

	public Demolishable(EventBus eventBus, EntityService entityService)
	{
		_eventBus = eventBus;
		_entityService = entityService;
	}

	public void Awake()
	{
		Reservable = GetComponent<Reservable>();
		_demolishJob = GetComponent<DemolishJob>();
		_demolishableSpec = GetComponent<DemolishableSpec>();
		_blockObject = GetComponent<BlockObject>();
		_demolishTimeLeft = _demolishableSpec.DemolishTimeInHours;
	}

	public void InitializeEntity()
	{
		if (_markPostLoad)
		{
			Mark();
		}
	}

	public void DeleteEntity()
	{
		Unmark();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (IsMarked)
		{
			IObjectSaver component = entitySaver.GetComponent(DemolishableKey);
			component.Set(IsMarkedKey, IsMarked);
			component.Set(DemolishTimeLeft, _demolishTimeLeft);
		}
	}

	[BackwardCompatible(2025, 8, 20, Compatibility.Map)]
	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(DemolishableKey, out var objectLoader))
		{
			_markPostLoad = objectLoader.Has(IsMarkedKey) && objectLoader.Get(IsMarkedKey);
			if (objectLoader.Has(DemolishTimeLeft))
			{
				_demolishTimeLeft = Math.Clamp(objectLoader.Get(DemolishTimeLeft), 0f, _demolishableSpec.DemolishTimeInHours);
			}
		}
	}

	public void DuplicateFrom(Demolishable source)
	{
		if (!IsMarked && source.IsMarked)
		{
			Mark();
		}
		else if (IsMarked && !source.IsMarked)
		{
			Unmark();
		}
	}

	public void Mark()
	{
		if (_blockObject.Overridable)
		{
			_entityService.Delete(this);
		}
		else if (!IsMarked)
		{
			IsMarked = true;
			_demolishJob.Enable();
			_blockObject.OverridableChanged += OnOverridableChanged;
			Marked?.Invoke(this, EventArgs.Empty);
			_eventBus.Post(new DemolishableMarkedEvent(this));
		}
	}

	public void Unmark()
	{
		if (IsMarked)
		{
			IsMarked = false;
			_demolishJob.Disable();
			_blockObject.OverridableChanged -= OnOverridableChanged;
			Unmarked?.Invoke(this, EventArgs.Empty);
			_eventBus.Post(new DemolishableUnmarkedEvent(this));
		}
	}

	public void ProgressDemolition(float deltaTime)
	{
		_demolishTimeLeft -= deltaTime;
	}

	private void OnOverridableChanged(object sender, bool overridable)
	{
		if (overridable)
		{
			_entityService.Delete(this);
		}
	}
}

using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WorkSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Demolishing;

public class Demolisher : BaseComponent, IAwakableComponent, IPersistentEntity, IDeletableEntity
{
	private static readonly ComponentKey DemolisherKey = new ComponentKey("Demolisher");

	private static readonly PropertyKey<ReservedDemolishable> ReservedDemolishableKey = new PropertyKey<ReservedDemolishable>("ReservedDemolishable");

	private readonly EntityService _entityService;

	private readonly ReservedDemolishableSerializer _reservedDemolishableSerializer;

	public ReservedDemolishable ReservedDemolishable { get; private set; }

	public Demolishable Demolishable => ReservedDemolishable.Demolishable;

	public bool HasReservedDemolishable => ReservedDemolishable != null;

	public event EventHandler<Demolishable> ReservedDemolishableChanged;

	public Demolisher(EntityService entityService, ReservedDemolishableSerializer reservedDemolishableSerializer)
	{
		_entityService = entityService;
		_reservedDemolishableSerializer = reservedDemolishableSerializer;
	}

	public void Awake()
	{
		GetComponent<Worker>().GotUnemployed += delegate
		{
			Unreserve();
		};
	}

	public void DeleteEntity()
	{
		Unreserve();
	}

	public bool IsReserved(Demolishable demolishable)
	{
		return demolishable == ReservedDemolishable?.Demolishable;
	}

	public void Reserve(Demolishable demolishable)
	{
		ReserveForDemolition(new ReservedDemolishable(demolishable, forceDemolish: false));
	}

	public void ReserveWithForcedDemolition(Demolishable demolishable)
	{
		ReserveForDemolition(new ReservedDemolishable(demolishable, forceDemolish: true));
	}

	public void Unreserve()
	{
		if (HasReservedDemolishable)
		{
			ReservedDemolishable.Demolishable.Unmarked -= OnDemolishableUnmarked;
			ReservedDemolishable.Demolishable.Reservable.Unreserve();
		}
		ReservedDemolishable = null;
		ReservedDemolishableChanged?.Invoke(this, null);
	}

	public void Demolish()
	{
		if (ReservedDemolishable.CanBeDemolished)
		{
			_entityService.Delete(ReservedDemolishable.Demolishable);
		}
		Unreserve();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (HasReservedDemolishable)
		{
			entitySaver.GetComponent(DemolisherKey).Set(ReservedDemolishableKey, ReservedDemolishable, _reservedDemolishableSerializer);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(DemolisherKey, out var objectLoader) && objectLoader.GetObsoletable(ReservedDemolishableKey, _reservedDemolishableSerializer, out var value))
		{
			ReserveForDemolition(value);
		}
	}

	private void ReserveForDemolition(ReservedDemolishable reservedDemolishable)
	{
		Unreserve();
		reservedDemolishable.Demolishable.Reservable.Reserve();
		reservedDemolishable.Demolishable.Unmarked += OnDemolishableUnmarked;
		ReservedDemolishable = reservedDemolishable;
		ReservedDemolishableChanged?.Invoke(this, reservedDemolishable.Demolishable);
	}

	private void OnDemolishableUnmarked(object sender, EventArgs e)
	{
		Unreserve();
	}
}

using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.Characters;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.EnterableSystem;

public class Enterer : BaseComponent, IAwakableComponent, IPostLoadableEntity, IPersistentEntity, IDeletableEntity
{
	private static readonly ComponentKey EntererKey = new ComponentKey("Enterer");

	private static readonly PropertyKey<Enterable> ReservedBuildingKey = new PropertyKey<Enterable>("ReservedBuilding");

	private static readonly PropertyKey<Enterable> CurrentBuildingKey = new PropertyKey<Enterable>("CurrentBuilding");

	private readonly ReferenceSerializer _referenceSerializer;

	private Character _character;

	private CharacterModel _characterModel;

	private Enterable _reservedBuilding;

	private Enterable _loadedReservedBuilding;

	private Enterable _loadedCurrentBuilding;

	public Enterable CurrentBuilding { get; private set; }

	public bool IsInside => CurrentBuilding != null;

	private bool HasReservedSlot => _reservedBuilding;

	public event EventHandler<EnteredEnterableEventArgs> EnteredEnterable;

	public event EventHandler ExitedEnterable;

	public event EventHandler EntererInitialized;

	public Enterer(ReferenceSerializer referenceSerializer)
	{
		_referenceSerializer = referenceSerializer;
	}

	public void Awake()
	{
		_character = GetComponent<Character>();
		_characterModel = GetComponent<CharacterModel>();
	}

	public void PostLoadEntity()
	{
		ResolveLoadedState();
	}

	public void DeleteEntity()
	{
		UnreserveSlotAndExit();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (HasReservedSlot || IsInside)
		{
			IObjectSaver component = entitySaver.GetComponent(EntererKey);
			if (HasReservedSlot)
			{
				component.Set(ReservedBuildingKey, _reservedBuilding, _referenceSerializer.Of<Enterable>());
			}
			if (IsInside)
			{
				component.Set(CurrentBuildingKey, CurrentBuilding, _referenceSerializer.Of<Enterable>());
			}
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(EntererKey, out var objectLoader))
		{
			if (objectLoader.Has(ReservedBuildingKey) && objectLoader.GetObsoletable(ReservedBuildingKey, _referenceSerializer.Of<Enterable>(), out var value))
			{
				_loadedReservedBuilding = value;
			}
			if (objectLoader.Has(CurrentBuildingKey) && objectLoader.GetObsoletable(CurrentBuildingKey, _referenceSerializer.Of<Enterable>(), out var value2))
			{
				_loadedCurrentBuilding = value2;
			}
		}
	}

	public void ReserveSlot(Enterable enterable)
	{
		UnreserveSlot();
		enterable.ReserveSlot();
		_reservedBuilding = enterable;
	}

	public void UnreserveSlot()
	{
		if (HasReservedSlot)
		{
			_reservedBuilding.UnreserveSlot();
			_reservedBuilding = null;
		}
	}

	public void Enter(Enterable enterable)
	{
		if ((bool)CurrentBuilding)
		{
			throw new InvalidOperationException($"{this} tried to enter {enterable} while already inside {CurrentBuilding}");
		}
		UnreserveSlot();
		if (enterable.CanEnter)
		{
			_characterModel.Hide();
			CurrentBuilding = enterable;
			enterable.Add(this);
			EnteredEnterable?.Invoke(this, new EnteredEnterableEventArgs(enterable));
		}
	}

	public void Exit()
	{
		if ((bool)CurrentBuilding)
		{
			CurrentBuilding.Remove(this);
			Abandon();
		}
	}

	public void Abandon()
	{
		Enterable currentBuilding = CurrentBuilding;
		CurrentBuilding = null;
		_characterModel.Rotation = currentBuilding.ExitWorldSpaceRotation;
		_characterModel.Show();
		if (currentBuilding != null)
		{
			ExitedEnterable?.Invoke(this, EventArgs.Empty);
		}
	}

	public void UnreserveSlotAndExit()
	{
		UnreserveSlot();
		Exit();
	}

	private void ResolveLoadedState()
	{
		if (_character.Alive)
		{
			if ((bool)_loadedReservedBuilding)
			{
				ReserveSlot(_loadedReservedBuilding);
			}
			if ((bool)_loadedCurrentBuilding)
			{
				Enter(_loadedCurrentBuilding);
			}
			EntererInitialized?.Invoke(this, EventArgs.Empty);
		}
	}
}

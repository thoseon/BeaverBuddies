using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntityNaming;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Characters;

public class Character : BaseComponent, IAwakableComponent, IPreInitializableEntity, IPersistentEntity, IChildhoodInfluenced
{
	private static readonly ComponentKey CharacterKey = new ComponentKey("Character");

	private static readonly PropertyKey<Vector3> PositionKey = new PropertyKey<Vector3>("Position");

	private static readonly PropertyKey<bool> AliveKey = new PropertyKey<bool>("Alive");

	private static readonly PropertyKey<int> DayOfBirthKey = new PropertyKey<int>("DayOfBirth");

	private readonly EventBus _eventBus;

	private readonly EntityService _entityService;

	private readonly IDayNightCycle _dayNightCycle;

	private NamedEntity _namedEntity;

	public bool Alive { get; private set; } = true;

	public int DayOfBirth { get; set; }

	public int Age => _dayNightCycle.DayNumber - DayOfBirth;

	public string FirstName => _namedEntity.EntityName;

	public event EventHandler Died;

	public Character(EventBus eventBus, EntityService entityService, IDayNightCycle dayNightCycle)
	{
		_eventBus = eventBus;
		_entityService = entityService;
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_namedEntity = GetComponent<NamedEntity>();
	}

	public void PreInitializeEntity()
	{
		if (!TryGetComponent<CharacterInit>(out var component))
		{
			return;
		}
		DayOfBirth = component.DayOfBirth;
		base.Transform.SetPositionAndRotation(component.Position, component.Rotation);
		if (!component.Child)
		{
			return;
		}
		foreach (IChildhoodInfluenced item in GetComponentsAllocating<IChildhoodInfluenced>())
		{
			item.InfluenceByChildhood(component.Child);
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(CharacterKey);
		component.Set(PositionKey, base.Transform.position);
		component.Set(AliveKey, Alive);
		component.Set(DayOfBirthKey, DayOfBirth);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(CharacterKey);
		base.Transform.position = component.Get(PositionKey);
		Alive = component.Get(AliveKey);
		DayOfBirth = component.Get(DayOfBirthKey);
	}

	public void InfluenceByChildhood(Character child)
	{
		DayOfBirth = child.DayOfBirth;
		_namedEntity.SetEntityName(child.FirstName);
	}

	public void DestroyCharacter()
	{
		KillCharacter();
		_entityService.Delete(this);
	}

	public void KillCharacter()
	{
		if (Alive)
		{
			Alive = false;
			Died?.Invoke(this, EventArgs.Empty);
			_eventBus.Post(new CharacterKilledEvent(this));
		}
	}
}

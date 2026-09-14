using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.EntityNaming;

public class NamedEntity : BaseComponent, IAwakableComponent, IPersistentEntity, IInitializableEntity
{
	private static readonly ComponentKey ComponentKey = new ComponentKey("NamedEntity");

	private static readonly PropertyKey<string> EntityNameKey = new PropertyKey<string>("EntityName");

	private static readonly Regex DigitsRegex = new Regex("\\d+", RegexOptions.Compiled);

	private EntityComponent _entityComponent;

	private NamedEntitySpec _namedEntitySpec;

	private readonly EventBus _eventBus;

	private List<IEntityNamer> _entityNamers;

	private string _sortableEntityName;

	public string EntityName { get; private set; }

	public bool IsEditable => _namedEntitySpec?.IsEditable ?? false;

	public NamedEntitySortingKey SortingKey => new NamedEntitySortingKey(_sortableEntityName ?? (_sortableEntityName = GenerateSortableEntityName()), _entityComponent.EntityId);

	public event EventHandler EntityNameChanged;

	public NamedEntity(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_entityComponent = GetComponent<EntityComponent>();
		_namedEntitySpec = GetComponent<NamedEntitySpec>();
		_entityNamers = GetComponentsAllocating<IEntityNamer>();
		if (_entityNamers.IsEmpty())
		{
			throw new Exception("A NamedEntity needs at least one IEntityNamer.");
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (IsEditable)
		{
			if (EntityName == null)
			{
				Debug.LogWarning($"Entity {_entityComponent?.EntityId} is editable but has no name.");
			}
			else
			{
				entitySaver.GetComponent(ComponentKey).Set(EntityNameKey, EntityName);
			}
		}
	}

	[BackwardCompatible(2026, 2, 3, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		if (IsEditable)
		{
			PropertyKey<string> key = new PropertyKey<string>("Name");
			PropertyKey<string> key2 = new PropertyKey<string>("DistrictName");
			if (entityLoader.TryGetComponent(ComponentKey, out var objectLoader) && objectLoader.Has(EntityNameKey))
			{
				SetEntityNameSilently(objectLoader.Get(EntityNameKey));
			}
			else if (entityLoader.TryGetComponent(new ComponentKey("Character"), out objectLoader) && objectLoader.Has(key))
			{
				SetEntityNameSilently(objectLoader.Get(key));
			}
			else if (entityLoader.TryGetComponent(new ComponentKey("DistrictCenter"), out objectLoader) && objectLoader.Has(key2))
			{
				SetEntityNameSilently(objectLoader.Get(key2));
			}
			else if (entityLoader.TryGetComponent(new ComponentKey("Automator"), out objectLoader) && objectLoader.Has(key))
			{
				SetEntityNameSilently(objectLoader.Get(key));
			}
			else
			{
				Debug.LogWarning("Editable NamedEntity '" + base.Name + "' was loaded without a name.");
			}
		}
	}

	public void InitializeEntity()
	{
		if (string.IsNullOrEmpty(EntityName))
		{
			SetEntityName(GetHighestPriorityNamer().GenerateEntityName());
		}
	}

	public void SetEntityName(string entityName)
	{
		if (!string.Equals(EntityName, entityName))
		{
			SetEntityNameSilently(entityName);
			EntityNameChanged?.Invoke(this, EventArgs.Empty);
			_eventBus.Post(new EntityNameChangedEvent(_entityComponent));
		}
	}

	private void SetEntityNameSilently(string entityName)
	{
		EntityName = entityName;
		_sortableEntityName = null;
	}

	private string GenerateSortableEntityName()
	{
		return DigitsRegex.Replace(EntityName, (Match digits) => digits.Value.PadLeft(5, '0'));
	}

	private IEntityNamer GetHighestPriorityNamer()
	{
		IEntityNamer entityNamer = _entityNamers[0];
		for (int i = 1; i < _entityNamers.Count; i++)
		{
			IEntityNamer entityNamer2 = _entityNamers[i];
			if (entityNamer2.EntityNamerPriority > entityNamer.EntityNamerPriority)
			{
				entityNamer = entityNamer2;
			}
		}
		return entityNamer;
	}
}

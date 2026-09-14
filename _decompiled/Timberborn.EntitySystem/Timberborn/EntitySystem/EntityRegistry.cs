using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.TemplateSystem;

namespace Timberborn.EntitySystem;

public class EntityRegistry
{
	private readonly Dictionary<Guid, EntityComponent> _entities = new Dictionary<Guid, EntityComponent>();

	private readonly List<EntityComponent> _entitiesInInstantiationOrder = new List<EntityComponent>();

	private readonly HashSet<string> _instantiatedEntityTemplates = new HashSet<string>();

	public ReadOnlyList<EntityComponent> Entities => _entitiesInInstantiationOrder.AsReadOnlyList();

	public void AddEntity(EntityComponent entityComponent)
	{
		_entities.Add(entityComponent.EntityId, entityComponent);
		_entitiesInInstantiationOrder.Add(entityComponent);
		RegisterInstantiatedTemplate(entityComponent);
	}

	public void RemoveEntity(EntityComponent entityComponent)
	{
		_entities.Remove(entityComponent.EntityId);
		_entitiesInInstantiationOrder.Remove(entityComponent);
	}

	public EntityComponent GetEntity(Guid entityId)
	{
		return _entities.GetOrDefault(entityId);
	}

	public bool WasTemplateInstantiated(string templateName)
	{
		return _instantiatedEntityTemplates.Contains(templateName);
	}

	private void RegisterInstantiatedTemplate(EntityComponent entityComponent)
	{
		string text = entityComponent.GetComponent<TemplateSpec>()?.TemplateName;
		if (text != null)
		{
			_instantiatedEntityTemplates.Add(text);
		}
	}
}

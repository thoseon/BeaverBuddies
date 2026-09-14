using Bindito.Core;
using Timberborn.BaseComponentSystem;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateInstantiation;
using UnityEngine;

namespace Timberborn.EntitySystem;

public class EntityService
{
	private readonly TemplateInstantiator _templateInstantiator;

	private readonly IContainer _container;

	private readonly EntityRegistry _entityRegistry;

	private readonly EventBus _eventBus;

	public EntityService(TemplateInstantiator templateInstantiator, IContainer container, EntityRegistry entityRegistry, EventBus eventBus)
	{
		_templateInstantiator = templateInstantiator;
		_container = container;
		_entityRegistry = entityRegistry;
		_eventBus = eventBus;
	}

	public EntityComponent Instantiate(Blueprint blueprint)
	{
		return Instantiate(new EntitySetup.Builder(blueprint));
	}

	public EntityComponent Instantiate(EntitySetup.Builder entitySetupBuilder)
	{
		EntityComponent instance = _container.GetInstance<EntityComponent>();
		entitySetupBuilder.AddInitComponent(instance);
		EntitySetup entitySetup = entitySetupBuilder.Build();
		instance.SetEntityId(entitySetup.Id);
		_templateInstantiator.Instantiate(entitySetup.Template, null, entitySetup.InitComponents);
		_entityRegistry.AddEntity(instance);
		_eventBus.Post(new EntityCreatedEvent(instance));
		if (entitySetup.ShouldInitialize)
		{
			instance.InitializeIfUninitialized();
		}
		return instance;
	}

	public void Delete(BaseComponent entity)
	{
		EntityComponent component = entity.GetComponent<EntityComponent>();
		component.Delete();
		_entityRegistry.RemoveEntity(component);
		Object.Destroy(component.GameObject);
	}
}

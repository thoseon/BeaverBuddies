using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.TemplateSystem;

namespace Timberborn.EntityUndoSystem;

public class UndoableEntityFactory
{
	private readonly EntityService _entityService;

	private readonly EntityRegistry _entityRegistry;

	private readonly TemplateNameMapper _templateNameMapper;

	private readonly UndoableEntitiesLoader _undoableEntitiesLoader;

	public UndoableEntityFactory(EntityService entityService, EntityRegistry entityRegistry, TemplateNameMapper templateNameMapper, UndoableEntitiesLoader undoableEntitiesLoader)
	{
		_entityService = entityService;
		_entityRegistry = entityRegistry;
		_templateNameMapper = templateNameMapper;
		_undoableEntitiesLoader = undoableEntitiesLoader;
	}

	public UndoableEntity CreateUninitialized(BaseComponent baseComponent)
	{
		EntityComponent component = baseComponent.GetComponent<EntityComponent>();
		return new UndoableEntity(_entityService, _entityRegistry, _templateNameMapper, _undoableEntitiesLoader, component.EntityId);
	}

	public UndoableEntity CreateInitialized(BaseComponent baseComponent)
	{
		UndoableEntity undoableEntity = CreateUninitialized(baseComponent);
		undoableEntity.InitializeUndoableState();
		return undoableEntity;
	}
}

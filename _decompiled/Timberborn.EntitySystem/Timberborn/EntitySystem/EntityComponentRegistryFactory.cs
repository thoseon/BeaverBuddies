namespace Timberborn.EntitySystem;

public class EntityComponentRegistryFactory
{
	private readonly RegisteredComponentService _registeredComponentService;

	public EntityComponentRegistryFactory(RegisteredComponentService registeredComponentService)
	{
		_registeredComponentService = registeredComponentService;
	}

	public EntityComponentRegistry Create()
	{
		return new EntityComponentRegistry(_registeredComponentService);
	}
}

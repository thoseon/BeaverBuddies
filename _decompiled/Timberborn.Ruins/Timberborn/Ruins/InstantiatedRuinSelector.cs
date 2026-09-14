using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.SelectionSystem;

namespace Timberborn.Ruins;

internal class InstantiatedRuinSelector : BaseComponent, IPostInitializableEntity
{
	private readonly EntitySelectionService _entitySelectionService;

	public InstantiatedRuinSelector(EntitySelectionService entitySelectionService)
	{
		_entitySelectionService = entitySelectionService;
	}

	public void PostInitializeEntity()
	{
		if (TryGetComponent<RuinInit>(out var component) && component.WasSelected)
		{
			_entitySelectionService.Select(GetComponent<SelectableObject>());
		}
	}
}

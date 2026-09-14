using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.NotificationSystem;
using Timberborn.SelectionSystem;

namespace Timberborn.Beavers;

internal class Adult : BaseComponent, IInitializableEntity
{
	private static readonly string GrewUpLocKey = "Beaver.GrewUp";

	private readonly NotificationBus _notificationBus;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly ILoc _loc;

	public Adult(NotificationBus notificationBus, EntitySelectionService entitySelectionService, ILoc loc)
	{
		_notificationBus = notificationBus;
		_entitySelectionService = entitySelectionService;
		_loc = loc;
	}

	public void InitializeEntity()
	{
		if (TryGetComponent<CharacterInit>(out var component) && component.Child != null)
		{
			ReplaceChild(component.Child);
		}
	}

	private void ReplaceChild(Character child)
	{
		SelectableObject component = child.GetComponent<SelectableObject>();
		SelectableObject component2 = GetComponent<SelectableObject>();
		_entitySelectionService.Replace(component, component2);
		child.DestroyCharacter();
		_notificationBus.Post(_loc.T(GrewUpLocKey, child.FirstName), this);
	}
}

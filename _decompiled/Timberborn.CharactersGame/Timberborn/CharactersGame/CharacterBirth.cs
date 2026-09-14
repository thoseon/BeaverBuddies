using Timberborn.BaseComponentSystem;
using Timberborn.DwellingSystem;
using Timberborn.EnterableSystem;
using Timberborn.EntityNaming;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.Localization;
using Timberborn.NotificationSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.CharactersGame;

public class CharacterBirth : BaseComponent, IPreInitializableEntity, IPostInitializableEntity
{
	private readonly NotificationBus _notificationBus;

	private readonly ILoc _loc;

	private readonly EventBus _eventBus;

	public CharacterBirth(NotificationBus notificationBus, ILoc loc, EventBus eventBus)
	{
		_notificationBus = notificationBus;
		_loc = loc;
		_eventBus = eventBus;
	}

	public void PreInitializeEntity()
	{
		if (TryGetComponent<CharacterBirthInit>(out var component))
		{
			if ((bool)component.DistrictCenter)
			{
				GetComponent<Citizen>().AssignInitialDistrict(component.DistrictCenter);
			}
			if ((bool)component.Dwelling)
			{
				component.Dwelling.AssignDweller(GetComponent<Dweller>());
				GetComponent<Enterer>().Enter(component.Dwelling.GetComponent<Enterable>());
			}
			if (component.EventToPost != null)
			{
				_eventBus.Post(component.EventToPost);
			}
		}
	}

	public void PostInitializeEntity()
	{
		if (HasComponent<CharacterBirthInit>())
		{
			_notificationBus.Post(_loc.T(GetComponent<CharacterBirthSpec>().NotificationLocKey, GetComponent<NamedEntity>().EntityName), this);
			_eventBus.Post(new CharacterBirthEvent(GetComponent<CharacterBirth>()));
		}
	}
}

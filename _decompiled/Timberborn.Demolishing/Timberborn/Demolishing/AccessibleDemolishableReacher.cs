using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.EntitySystem;
using Timberborn.Navigation;
using Timberborn.WalkingSystem;
using UnityEngine;

namespace Timberborn.Demolishing;

public class AccessibleDemolishableReacher : DemolishableReacher, IPostInitializableEntity
{
	private IDestination _destination;

	public override IDestination Destination => _destination;

	public void PostInitializeEntity()
	{
		_destination = new AccessibleDestination(GetEnabledComponent<Accessible>());
	}

	public override void NotifyReservableReached(BaseComponent agent)
	{
		BlockObjectCenter component = GetComponent<BlockObjectCenter>();
		Vector3 target = (component ? component.WorldCenterGrounded : base.Transform.position);
		agent.GetComponent<CharacterModel>().LookToward(target);
	}
}

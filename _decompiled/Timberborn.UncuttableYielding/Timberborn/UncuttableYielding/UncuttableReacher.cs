using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.Demolishing;
using Timberborn.EntitySystem;
using Timberborn.NaturalResourcesModelSystem;
using Timberborn.WalkingSystem;
using UnityEngine;

namespace Timberborn.UncuttableYielding;

internal class UncuttableReacher : DemolishableReacher, IInitializableEntity
{
	private readonly PositionDestinationFactory _positionDestinationFactory;

	private IDestination _destination;

	private Vector3 _center;

	public override IDestination Destination => _destination;

	public UncuttableReacher(PositionDestinationFactory positionDestinationFactory)
	{
		_positionDestinationFactory = positionDestinationFactory;
	}

	public void InitializeEntity()
	{
		_center = GetComponent<NaturalResourceCenterProvider>().GetWorldCenter();
		_destination = _positionDestinationFactory.Create(_center, 0.5f);
	}

	public override void NotifyReservableReached(BaseComponent agent)
	{
		agent.GetComponent<CharacterModel>().LookToward(_center);
	}
}

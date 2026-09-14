using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.EntitySystem;
using Timberborn.NaturalResourcesModelSystem;
using Timberborn.ReservableSystem;
using Timberborn.WalkingSystem;
using UnityEngine;

namespace Timberborn.Forestry;

internal class TreeReacher : ReservableReacher, IInitializableEntity
{
	private static readonly float CuttingPositionOffset = 0.182f;

	private readonly PositionDestinationFactory _positionDestinationFactory;

	private PositionDestination _destination;

	public override IDestination Destination => _destination;

	public TreeReacher(PositionDestinationFactory positionDestinationFactory)
	{
		_positionDestinationFactory = positionDestinationFactory;
	}

	public void InitializeEntity()
	{
		Vector3 worldCenter = GetComponent<NaturalResourceCenterProvider>().GetWorldCenter();
		float diameterScale = GetComponent<NaturalResourceModelRandomizer>().DiameterScale;
		float stoppingDistance = GetComponent<TreeCuttingRadiusSpec>().Radius * diameterScale + CuttingPositionOffset;
		_destination = _positionDestinationFactory.Create(worldCenter, stoppingDistance);
	}

	public override void NotifyReservableReached(BaseComponent agent)
	{
		CharacterModel component = agent.GetComponent<CharacterModel>();
		component.LookToward(_destination.Destination);
		Vector3 position = component.Transform.position;
		Vector3 vector = _destination.Destination - position;
		Vector3 vector2 = vector.normalized * _destination.StoppingDistance;
		Vector3 vector3 = vector - vector2;
		component.Transform.position = position + vector3;
	}
}

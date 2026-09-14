using System;
using Timberborn.Navigation;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WalkingSystem;

public class DestinationValueSerializer : IValueSerializer<IDestination>
{
	private static readonly PropertyKey<string> ImplementationTypeKey = new PropertyKey<string>("ImplementationType");

	private static readonly PropertyKey<Vector3> PositionKey = new PropertyKey<Vector3>("Position");

	private static readonly PropertyKey<float> StoppingDistanceKey = new PropertyKey<float>("StoppingDistance");

	private static readonly PropertyKey<Accessible> AccessibleKey = new PropertyKey<Accessible>("Accessible");

	private readonly ReferenceSerializer _referenceSerializer;

	private readonly PositionDestinationFactory _positionDestinationFactory;

	public DestinationValueSerializer(ReferenceSerializer referenceSerializer, PositionDestinationFactory positionDestinationFactory)
	{
		_positionDestinationFactory = positionDestinationFactory;
		_referenceSerializer = referenceSerializer;
	}

	public void Serialize(IDestination value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		if (!(value is PositionDestination positionDestination))
		{
			if (!(value is AccessibleDestination accessibleDestination))
			{
				throw new ArgumentOutOfRangeException("Unknown IDestination implementation");
			}
			ConvertAccessibleDestination(accessibleDestination, objectSaver);
		}
		else
		{
			ConvertPositionDestination(positionDestination, objectSaver);
		}
	}

	public Obsoletable<IDestination> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		string text = objectLoader.Get(ImplementationTypeKey);
		if (!(text == "PositionDestination"))
		{
			if (text == "AccessibleDestination")
			{
				return DeconvertAccessibleDestination(objectLoader);
			}
			throw new ArgumentOutOfRangeException("Unknown IDestination implementation");
		}
		return DeconvertPositionDestination(objectLoader);
	}

	private static void ConvertPositionDestination(PositionDestination positionDestination, IObjectSaver objectSaver)
	{
		objectSaver.Set(ImplementationTypeKey, "PositionDestination");
		objectSaver.Set(PositionKey, positionDestination.Destination);
		objectSaver.Set(StoppingDistanceKey, positionDestination.StoppingDistance);
	}

	private Obsoletable<IDestination> DeconvertPositionDestination(IObjectLoader objectLoader)
	{
		Vector3 position = objectLoader.Get(PositionKey);
		float stoppingDistance = objectLoader.Get(StoppingDistanceKey);
		return _positionDestinationFactory.Create(position, stoppingDistance);
	}

	private void ConvertAccessibleDestination(AccessibleDestination accessibleDestination, IObjectSaver objectSaver)
	{
		objectSaver.Set(ImplementationTypeKey, "AccessibleDestination");
		if ((bool)accessibleDestination.Accessible)
		{
			objectSaver.Set(AccessibleKey, accessibleDestination.Accessible, _referenceSerializer.Of<Accessible>());
		}
	}

	private Obsoletable<IDestination> DeconvertAccessibleDestination(IObjectLoader objectLoader)
	{
		if (objectLoader.Has(AccessibleKey) && objectLoader.GetObsoletable(AccessibleKey, _referenceSerializer.Of<Accessible>(), out var value) && (bool)value)
		{
			return new AccessibleDestination(value);
		}
		return null;
	}
}

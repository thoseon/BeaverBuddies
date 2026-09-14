using Timberborn.Persistence;
using UnityEngine;

namespace Timberborn.Explosions;

public class ExplosionDataValueSerializer : IValueSerializer<ExplosionData>
{
	private static readonly PropertyKey<float> RadiusKey = new PropertyKey<float>("Radius");

	private static readonly PropertyKey<Vector3> CenterKey = new PropertyKey<Vector3>("Center");

	private static readonly PropertyKey<int> CurrentExplosionRadiusKey = new PropertyKey<int>("CurrentExplosionRadius");

	public void Serialize(ExplosionData value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(RadiusKey, value.Radius);
		objectSaver.Set(CenterKey, value.Center);
		objectSaver.Set(CurrentExplosionRadiusKey, value.CurrentExplosionRadius);
	}

	public Obsoletable<ExplosionData> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		float radius = objectLoader.Get(RadiusKey);
		Vector3 center = objectLoader.Get(CenterKey);
		int currentExplosionRadius = objectLoader.Get(CurrentExplosionRadiusKey);
		return new ExplosionData(radius, center, currentExplosionRadius);
	}
}

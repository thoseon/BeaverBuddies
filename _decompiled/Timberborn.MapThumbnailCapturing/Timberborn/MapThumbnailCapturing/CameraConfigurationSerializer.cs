using Timberborn.Persistence;
using UnityEngine;

namespace Timberborn.MapThumbnailCapturing;

public class CameraConfigurationSerializer : IValueSerializer<CameraConfiguration>
{
	private static readonly PropertyKey<Vector3> PositionKey = new PropertyKey<Vector3>("Position");

	private static readonly PropertyKey<Quaternion> RotationKey = new PropertyKey<Quaternion>("Rotation");

	private static readonly PropertyKey<float> ShadowDistanceKey = new PropertyKey<float>("ShadowDistance");

	public void Serialize(CameraConfiguration value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(PositionKey, value.Position);
		objectSaver.Set(RotationKey, value.Rotation);
		objectSaver.Set(ShadowDistanceKey, value.ShadowDistance);
	}

	public Obsoletable<CameraConfiguration> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		return new Obsoletable<CameraConfiguration>(new CameraConfiguration(objectLoader.Get(PositionKey), objectLoader.Get(RotationKey), objectLoader.Get(ShadowDistanceKey)));
	}
}

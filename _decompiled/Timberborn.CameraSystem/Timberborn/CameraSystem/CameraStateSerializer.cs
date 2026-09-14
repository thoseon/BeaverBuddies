using Timberborn.Persistence;
using UnityEngine;

namespace Timberborn.CameraSystem;

public class CameraStateSerializer : IValueSerializer<CameraState>
{
	private static readonly PropertyKey<Vector3> TargetKey = new PropertyKey<Vector3>("Target");

	private static readonly PropertyKey<float> ZoomLevelKey = new PropertyKey<float>("ZoomLevel");

	private static readonly PropertyKey<float> HorizontalAngleKey = new PropertyKey<float>("HorizontalAngle");

	private static readonly PropertyKey<float> VerticalAngleKey = new PropertyKey<float>("VerticalAngle");

	public void Serialize(CameraState value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(TargetKey, value.Target);
		objectSaver.Set(ZoomLevelKey, value.ZoomLevel);
		objectSaver.Set(HorizontalAngleKey, value.HorizontalAngle);
		objectSaver.Set(VerticalAngleKey, value.VerticalAngle);
	}

	public Obsoletable<CameraState> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		return new CameraState(objectLoader.Get(TargetKey), objectLoader.Get(ZoomLevelKey), objectLoader.Get(HorizontalAngleKey), objectLoader.Get(VerticalAngleKey));
	}
}

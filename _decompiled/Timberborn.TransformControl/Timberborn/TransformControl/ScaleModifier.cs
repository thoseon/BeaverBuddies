using UnityEngine;

namespace Timberborn.TransformControl;

public class ScaleModifier
{
	private readonly TransformController _transformController;

	public Vector3 Value { get; private set; } = Vector3.one;

	internal ScaleModifier(TransformController transformController)
	{
		_transformController = transformController;
	}

	public void Set(Vector3 value)
	{
		if (!value.Equals(Value))
		{
			Value = value;
			_transformController.ApplyScale();
		}
	}

	public void Reset()
	{
		Set(Vector3.one);
	}
}

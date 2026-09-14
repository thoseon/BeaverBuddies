using UnityEngine;

namespace Timberborn.TransformControl;

public class PositionModifier
{
	private readonly TransformController _transformController;

	public Vector3 Value { get; private set; }

	internal PositionModifier(TransformController transformController)
	{
		_transformController = transformController;
	}

	public void Set(Vector3 value)
	{
		if (!value.Equals(Value))
		{
			Value = value;
			_transformController.ApplyPosition();
		}
	}

	public void Reset()
	{
		Set(Vector3.zero);
	}
}

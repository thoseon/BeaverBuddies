using UnityEngine;

namespace Timberborn.TransformControl;

public class RotationModifier
{
	private readonly TransformController _transformController;

	public Quaternion Value { get; private set; } = Quaternion.identity;

	internal RotationModifier(TransformController transformController)
	{
		_transformController = transformController;
	}

	public void Set(Quaternion value)
	{
		if (!value.Equals(Value))
		{
			Value = value;
			_transformController.ApplyRotation();
		}
	}

	public void Reset()
	{
		Set(Quaternion.identity);
	}
}

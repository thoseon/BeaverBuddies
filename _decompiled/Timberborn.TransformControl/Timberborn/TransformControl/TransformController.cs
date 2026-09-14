using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using UnityEngine;

namespace Timberborn.TransformControl;

public class TransformController : BaseComponent
{
	private readonly List<PositionModifier> _positionModifiers = new List<PositionModifier>();

	private readonly SortedList<int, RotationModifier> _rotationModifiers = new SortedList<int, RotationModifier>();

	private readonly List<ScaleModifier> _scaleModifiers = new List<ScaleModifier>();

	public PositionModifier AddPositionModifier()
	{
		PositionModifier positionModifier = new PositionModifier(this);
		_positionModifiers.Add(positionModifier);
		return positionModifier;
	}

	public ScaleModifier AddScaleModifier()
	{
		ScaleModifier scaleModifier = new ScaleModifier(this);
		_scaleModifiers.Add(scaleModifier);
		return scaleModifier;
	}

	public RotationModifier AddRotationModifier(int order)
	{
		RotationModifier rotationModifier = new RotationModifier(this);
		if (_rotationModifiers.TryAdd(order, rotationModifier))
		{
			return rotationModifier;
		}
		throw new ArgumentException($"A rotation modifier with order {order} already exists.");
	}

	internal void ApplyPosition()
	{
		base.Transform.localPosition = CalculatePosition();
	}

	internal void ApplyRotation()
	{
		base.Transform.localRotation = CalculateRotation();
	}

	internal void ApplyScale()
	{
		base.Transform.localScale = CalculateScale();
	}

	private Vector3 CalculatePosition()
	{
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < _positionModifiers.Count; i++)
		{
			zero += _positionModifiers[i].Value;
		}
		return zero;
	}

	private Quaternion CalculateRotation()
	{
		Quaternion quaternion = Quaternion.identity;
		IList<RotationModifier> values = _rotationModifiers.Values;
		for (int i = 0; i < values.Count; i++)
		{
			quaternion = values[i].Value * quaternion;
		}
		return quaternion;
	}

	private Vector3 CalculateScale()
	{
		Vector3 vector = Vector3.one;
		for (int i = 0; i < _scaleModifiers.Count; i++)
		{
			vector = Vector3.Scale(vector, _scaleModifiers[i].Value);
		}
		return vector;
	}
}

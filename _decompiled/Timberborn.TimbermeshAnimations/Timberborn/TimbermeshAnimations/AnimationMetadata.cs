using System;
using UnityEngine;

namespace Timberborn.TimbermeshAnimations;

[Serializable]
internal class AnimationMetadata
{
	[SerializeField]
	private string _name;

	[SerializeField]
	private float _length;

	public string Name => _name;

	public float Length => _length;

	public AnimationMetadata(string name, float length)
	{
		_name = name;
		_length = length;
	}
}

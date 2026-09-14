using System;
using UnityEngine;

namespace Timberborn.TimbermeshAnimations;

[Serializable]
internal class VertexAnimation
{
	[SerializeField]
	private string _name;

	[SerializeField]
	private int _frameCount;

	[SerializeField]
	private int _animatedVertexCount;

	[SerializeField]
	private Texture _offsets;

	[SerializeField]
	private Texture _rotations;

	public string Name => _name;

	public int AnimatedVertexCount => _animatedVertexCount;

	public int FrameCount => _frameCount;

	public Texture Offsets => _offsets;

	public Texture Rotations => _rotations;

	public VertexAnimation(string name, int frameCount, int animatedVertexCount, Texture offsets, Texture rotations)
	{
		_name = name;
		_frameCount = frameCount;
		_animatedVertexCount = animatedVertexCount;
		_offsets = offsets;
		_rotations = rotations;
	}
}

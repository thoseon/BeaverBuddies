using System;
using Timberborn.TimbermeshDTO;
using UnityEngine;

namespace Timberborn.TimbermeshEditorTools;

[Serializable]
public class NodeAnimationMetadata
{
	[SerializeField]
	private string _nodeName;

	[SerializeField]
	private string _animationName;

	[SerializeField]
	private float _framerate;

	[SerializeField]
	private int _frameCount;

	public string NodeName => _nodeName;

	public string AnimationName => _animationName;

	public float Framerate => _framerate;

	public int FrameCount => _frameCount;

	public NodeAnimationMetadata(string nodeName, NodeAnimation animation)
	{
		_nodeName = nodeName;
		_animationName = animation.Name;
		_framerate = animation.Framerate;
		_frameCount = animation.Frames.Count;
	}
}

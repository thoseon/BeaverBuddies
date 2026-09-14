using System;
using System.Collections.Generic;
using Timberborn.Timbermesh;
using Timberborn.TimbermeshDTO;
using UnityEngine;

namespace Timberborn.TimbermeshAnimations;

internal class NodeAnimationCache
{
	private readonly Dictionary<int, List<NodeAnimation>> _animations = new Dictionary<int, List<NodeAnimation>>();

	private int _lastAnimationsId;

	public int CacheAnimations(Node sourceNode)
	{
		int num = ++_lastAnimationsId;
		List<NodeAnimation> list = new List<NodeAnimation>();
		foreach (Timberborn.TimbermeshDTO.NodeAnimation nodeAnimation in sourceNode.NodeAnimations)
		{
			if (nodeAnimation.Frames.Count == 0)
			{
				throw new InvalidOperationException("Animation " + nodeAnimation.Name + " in node " + sourceNode.Name + " has no frames.");
			}
			NodeAnimation item = CreateNodeAnimation(nodeAnimation);
			list.Add(item);
		}
		_animations.Add(num, list);
		return num;
	}

	public IEnumerable<NodeAnimation> GetAnimations(int animationsId)
	{
		return _animations[animationsId];
	}

	private static NodeAnimation CreateNodeAnimation(Timberborn.TimbermeshDTO.NodeAnimation animation)
	{
		int count = animation.Frames.Count;
		Vector3[] array = new Vector3[count];
		Quaternion[] array2 = new Quaternion[count];
		Vector3[] array3 = new Vector3[count];
		for (int i = 0; i < count; i++)
		{
			NodeAnimationFrame nodeAnimationFrame = animation.Frames[i];
			array[i] = nodeAnimationFrame.Position.ToVector3();
			array2[i] = nodeAnimationFrame.Rotation.ToQuaternion();
			array3[i] = nodeAnimationFrame.Scale.ToVector3();
		}
		return NodeAnimation.Create(animation.Name, count, array, array2, array3);
	}
}

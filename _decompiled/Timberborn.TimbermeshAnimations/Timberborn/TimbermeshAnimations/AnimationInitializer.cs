using System.Collections.Generic;
using System.Linq;
using Bindito.Unity;
using Timberborn.Timbermesh;
using Timberborn.TimbermeshDTO;
using UnityEngine;

namespace Timberborn.TimbermeshAnimations;

internal class AnimationInitializer : IModelPostprocessor
{
	private readonly IInstantiator _instantiator;

	private readonly VertexAnimationInitializer _vertexAnimationInitializer;

	private readonly NodeAnimationInitializer _nodeAnimationInitializer;

	private readonly Dictionary<string, AnimationMetadata> _animationMap = new Dictionary<string, AnimationMetadata>();

	public AnimationInitializer(IInstantiator instantiator, VertexAnimationInitializer vertexAnimationInitializer, NodeAnimationInitializer nodeAnimationInitializer)
	{
		_instantiator = instantiator;
		_vertexAnimationInitializer = vertexAnimationInitializer;
		_nodeAnimationInitializer = nodeAnimationInitializer;
	}

	public void Postprocess(ImportDetails details)
	{
		InitializeAllAnimations(details);
		if (_animationMap.Any())
		{
			AddAnimator(details.Root.gameObject);
			_animationMap.Clear();
		}
	}

	private void InitializeAllAnimations(ImportDetails importDetails)
	{
		foreach (var (node2, animatedObject) in importDetails.CreatedObjectsMap)
		{
			AddAnimations(node2.VertexAnimations);
			AddAnimations(node2.NodeAnimations);
			_vertexAnimationInitializer.InitializeAnimations(animatedObject, node2);
			_nodeAnimationInitializer.InitializeAnimations(animatedObject, node2);
		}
	}

	private void AddAnimations(IReadOnlyList<IAnimation> animations)
	{
		for (int i = 0; i < animations.Count; i++)
		{
			IAnimation animation = animations[i];
			if (!_animationMap.ContainsKey(animation.Name))
			{
				_animationMap.Add(animation.Name, new AnimationMetadata(animation.Name, animation.Length));
			}
		}
	}

	private void AddAnimator(GameObject animatedObject)
	{
		_instantiator.AddComponent<TimbermeshAnimator>(animatedObject).SetAnimations(_animationMap.Values);
	}
}

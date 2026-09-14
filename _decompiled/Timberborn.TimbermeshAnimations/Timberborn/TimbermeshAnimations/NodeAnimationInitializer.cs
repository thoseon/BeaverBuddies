using System.Linq;
using Bindito.Unity;
using Timberborn.TimbermeshDTO;
using UnityEngine;

namespace Timberborn.TimbermeshAnimations;

internal class NodeAnimationInitializer
{
	private readonly IInstantiator _instantiator;

	private readonly NodeAnimationCache _nodeAnimationCache;

	public NodeAnimationInitializer(IInstantiator instantiator, NodeAnimationCache nodeAnimationCache)
	{
		_instantiator = instantiator;
		_nodeAnimationCache = nodeAnimationCache;
	}

	public void InitializeAnimations(GameObject animatedObject, Node source)
	{
		if (source.NodeAnimations.Any())
		{
			int animationSetId = _nodeAnimationCache.CacheAnimations(source);
			_instantiator.AddComponent<NodeAnimationUpdater>(animatedObject).AssignAnimationsId(animationSetId);
		}
	}
}

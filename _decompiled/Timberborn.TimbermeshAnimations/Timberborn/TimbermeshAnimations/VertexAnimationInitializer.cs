using System.Collections.Generic;
using System.Linq;
using Bindito.Unity;
using Timberborn.TimbermeshDTO;
using UnityEngine;

namespace Timberborn.TimbermeshAnimations;

internal class VertexAnimationInitializer
{
	private readonly IInstantiator _instantiator;

	private readonly VertexAnimationTextureGenerator _vertexAnimationTextureGenerator;

	public VertexAnimationInitializer(IInstantiator instantiator, VertexAnimationTextureGenerator vertexAnimationTextureGenerator)
	{
		_instantiator = instantiator;
		_vertexAnimationTextureGenerator = vertexAnimationTextureGenerator;
	}

	public void InitializeAnimations(GameObject animatedObject, Node source)
	{
		if (source.VertexCount > 0)
		{
			List<VertexAnimation> list = CreateAnimations(source);
			if (list.Any())
			{
				_instantiator.AddComponent<VertexAnimationUpdater>(animatedObject).AssignAnimations(list);
			}
		}
	}

	private List<VertexAnimation> CreateAnimations(Node source)
	{
		List<VertexAnimation> list = new List<VertexAnimation>();
		foreach (Timberborn.TimbermeshDTO.VertexAnimation vertexAnimation in source.VertexAnimations)
		{
			(Texture, Texture) tuple = _vertexAnimationTextureGenerator.CreateAnimationTextures(vertexAnimation);
			Texture item = tuple.Item1;
			Texture item2 = tuple.Item2;
			VertexAnimation item3 = new VertexAnimation(vertexAnimation.Name, vertexAnimation.Frames.Count, vertexAnimation.AnimatedVertexCount, item, item2);
			list.Add(item3);
		}
		return list;
	}
}

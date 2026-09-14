using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Timberborn.TimbermeshAnimations;

internal class VertexAnimationUpdater : MonoBehaviour, IAnimationUpdater
{
	private static readonly int OffsetsId = Shader.PropertyToID("_Offsets");

	private static readonly int RotationsId = Shader.PropertyToID("_Rotations");

	private static readonly int AnimatedVertexCountId = Shader.PropertyToID("_AnimatedVertexCount");

	private static readonly int FrameCountId = Shader.PropertyToID("_FrameCount");

	private static readonly int Looped = Shader.PropertyToID("_Looped");

	private static readonly int AnimationTimeId = Shader.PropertyToID("_AnimationTime");

	private static readonly int AnimationEnabledId = Shader.PropertyToID("_AnimationEnabled");

	[HideInInspector]
	[SerializeField]
	private List<VertexAnimation> _animations;

	private Dictionary<string, VertexAnimation> _animationsMap;

	private VertexAnimation _currentAnimation;

	private MeshRenderer _meshRenderer;

	private Material Material => _meshRenderer.material;

	public void Initialize()
	{
		_animationsMap = _animations.ToDictionary((VertexAnimation a) => a.Name, (VertexAnimation a) => a);
		SetupMeshRenderer();
	}

	public void AssignAnimations(List<VertexAnimation> animations)
	{
		_animations = animations;
	}

	public void SetAnimation(string animationName, bool looped)
	{
		_currentAnimation = null;
		if (_animationsMap.TryGetValue(animationName, out var value))
		{
			_currentAnimation = value;
			UpdateMaterialProperties(looped);
		}
	}

	public void UpdateAnimation(float normalizedTime)
	{
		if (_currentAnimation != null)
		{
			Material.SetFloat(AnimationTimeId, normalizedTime);
		}
	}

	private void SetupMeshRenderer()
	{
		_meshRenderer = GetComponent<MeshRenderer>();
		Material.SetFloat(AnimationEnabledId, 1f);
	}

	private void UpdateMaterialProperties(bool looped)
	{
		Material.SetFloat(AnimatedVertexCountId, _currentAnimation.AnimatedVertexCount);
		Material.SetFloat(FrameCountId, _currentAnimation.FrameCount);
		Material.SetFloat(Looped, looped ? 1f : 0f);
		Material.SetTexture(OffsetsId, _currentAnimation.Offsets);
		Material.SetTexture(RotationsId, _currentAnimation.Rotations);
	}
}

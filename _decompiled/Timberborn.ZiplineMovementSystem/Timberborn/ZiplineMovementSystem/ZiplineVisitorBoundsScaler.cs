using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using UnityEngine;

namespace Timberborn.ZiplineMovementSystem;

internal class ZiplineVisitorBoundsScaler : BaseComponent, IAwakableComponent
{
	private static readonly float Scale = 4f;

	private ImmutableArray<MeshRenderer> _meshRenderers;

	public void Awake()
	{
		_meshRenderers = ((IEnumerable<MeshRenderer>)base.GameObject.GetComponentsInChildren<MeshRenderer>(includeInactive: true)).ToImmutableArray();
		ZiplineVisitor component = GetComponent<ZiplineVisitor>();
		component.EnteredZipline += OnEnteredZipline;
		component.ExitedZipline += OnExitedZipline;
	}

	private void OnEnteredZipline(object sender, EventArgs e)
	{
		foreach (MeshRenderer meshRenderer in _meshRenderers)
		{
			Bounds localBounds = meshRenderer.localBounds;
			localBounds.size *= Scale;
			meshRenderer.localBounds = localBounds;
		}
	}

	private void OnExitedZipline(object sender, EventArgs e)
	{
		foreach (MeshRenderer meshRenderer in _meshRenderers)
		{
			meshRenderer.ResetLocalBounds();
		}
	}
}

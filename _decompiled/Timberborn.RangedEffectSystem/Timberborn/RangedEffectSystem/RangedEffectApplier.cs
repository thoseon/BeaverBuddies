using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Effects;
using Timberborn.NeedSpecs;
using UnityEngine;

namespace Timberborn.RangedEffectSystem;

internal class RangedEffectApplier : BaseComponent, IAwakableComponent
{
	private readonly RangedEffectService _rangedEffectService;

	private ImmutableArray<Vector2Int> _effectAreaCoords;

	public bool Active { get; private set; }

	public float Efficiency { get; private set; } = 1f;

	public ImmutableArray<ContinuousEffect> Effects { get; private set; }

	public event EventHandler<ActiveChangedEventArgs> ActiveChanged;

	public RangedEffectApplier(RangedEffectService rangedEffectService)
	{
		_rangedEffectService = rangedEffectService;
	}

	public void Awake()
	{
		DisableComponent();
	}

	public void Enable(IEnumerable<ContinuousEffectSpec> specs, IEnumerable<Vector2Int> radius, bool active)
	{
		Effects = specs.Select(ContinuousEffect.FromSpec).ToImmutableArray();
		_effectAreaCoords = radius.ToImmutableArray();
		_rangedEffectService.SetApplier(this);
		EnableComponent();
		UpdateActiveState(active);
	}

	public void Disable()
	{
		_rangedEffectService.UnsetApplier(this);
		DisableComponent();
	}

	public void UpdateActiveState(bool active)
	{
		Active = active;
		ActiveChanged?.Invoke(this, new ActiveChangedEventArgs(active));
	}

	public void UpdateEfficiency(float efficiency)
	{
		Efficiency = efficiency;
	}

	public IEnumerable<Vector2Int> EffectAreaCoords()
	{
		return _effectAreaCoords;
	}
}

using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.WorldPersistence;
using Timberborn.Yielding;
using UnityEngine;

namespace Timberborn.Ruins;

public class Ruin : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private readonly RuinReplacer _ruinReplacer;

	private RuinSpec _ruinSpec;

	public Yielder Yielder { get; private set; }

	public Transform ModelParent { get; private set; }

	public int SpecifiedHeight => _ruinSpec.RuinHeight;

	public YielderSpec YielderSpec => _ruinSpec.Yielder;

	public Ruin(RuinReplacer ruinReplacer)
	{
		_ruinReplacer = ruinReplacer;
	}

	public void Awake()
	{
		_ruinSpec = GetComponent<RuinSpec>();
		Yielder = this.GetNamedComponent<Yielder>(YielderSpec.YielderComponentName);
		ModelParent = base.GameObject.FindChildTransform(_ruinSpec.ModelParentName);
	}

	public void InitializeEntity()
	{
		Yielder.YieldDecreased += OnYieldDecreased;
		Yielder.Enable();
		if (TryGetComponent<RuinInit>(out var component))
		{
			ResolveInitialYield(component);
		}
	}

	private void ResolveInitialYield(RuinInit ruinInit)
	{
		if (ruinInit.InitialYield.HasValue)
		{
			int num = Yielder.Yield.Amount - ruinInit.InitialYield.Value;
			if (num != 0)
			{
				Yielder.DecreaseYield(new GoodAmount(Yielder.YielderSpec.Yield.Id, num));
			}
		}
	}

	private void OnYieldDecreased(object sender, EventArgs e)
	{
		UpdateHeight();
	}

	private void UpdateHeight()
	{
		int num = Mathf.CeilToInt((float)YielderSpec.Yield.Amount / (float)SpecifiedHeight);
		if (Mathf.CeilToInt((float)Yielder.Yield.Amount / (float)num) != SpecifiedHeight)
		{
			_ruinReplacer.Shrink(this);
		}
	}
}

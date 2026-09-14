using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Characters;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.Effects;
using Timberborn.NeedSystem;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.NeedApplication;

public class AreaNeedApplier : BaseComponent, IAwakableComponent, IFinishedStateListener, IPersistentEntity, IProbabilityGroupProvider
{
	private static readonly float CheckIntervalInDays = 1f / 24f;

	private static readonly ComponentKey AreaNeedApplierKey = new ComponentKey("AreaNeedApplier");

	private static readonly PropertyKey<float> ApplicationTriggerProgressKey = new PropertyKey<float>("ApplicationTriggerProgress");

	private readonly EffectProbabilityService _effectProbabilityService;

	private readonly ITimeTriggerFactory _timeTriggerFactory;

	private readonly CharacterPopulation _characterPopulation;

	private BlockObjectRange _blockObjectRange;

	private AreaNeedApplierSpec _areaNeedApplierSpec;

	private ITimeTrigger _applicationTrigger;

	private IEnumerable<Vector2Int> _influencedBlocks;

	public string ProbabilityGroupId => "AreaNeedApplier";

	public event EventHandler<NeedAppliedEventArgs> NeedApplied;

	public AreaNeedApplier(EffectProbabilityService effectProbabilityService, ITimeTriggerFactory timeTriggerFactory, CharacterPopulation characterPopulation)
	{
		_effectProbabilityService = effectProbabilityService;
		_timeTriggerFactory = timeTriggerFactory;
		_characterPopulation = characterPopulation;
	}

	public void Awake()
	{
		_blockObjectRange = GetComponent<BlockObjectRange>();
		_areaNeedApplierSpec = GetComponent<AreaNeedApplierSpec>();
		_applicationTrigger = _timeTriggerFactory.Create(TryApplyNeed, CheckIntervalInDays);
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		UpdateInfluencedBlocks();
		_applicationTrigger.Resume();
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		_applicationTrigger.Pause();
		DisableComponent();
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(AreaNeedApplierKey).Set(ApplicationTriggerProgressKey, _applicationTrigger.Progress);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(AreaNeedApplierKey);
		_applicationTrigger.FastForwardProgress(component.Get(ApplicationTriggerProgressKey));
	}

	private void TryApplyNeed()
	{
		foreach (NeedApplierEffectSpec effect in _areaNeedApplierSpec.Effects)
		{
			for (int i = 0; i < _characterPopulation.Characters.Count; i++)
			{
				if (_effectProbabilityService.CanApply(this, effect))
				{
					Character character = _characterPopulation.Characters[i];
					Vector3Int value = CoordinateSystem.WorldToGridInt(character.Transform.position);
					if (IsInfluencedByApplier(value.XY()))
					{
						ApplyNeed(character, effect);
					}
				}
			}
		}
		_applicationTrigger.Reset();
		_applicationTrigger.Resume();
	}

	private void UpdateInfluencedBlocks()
	{
		_influencedBlocks = _blockObjectRange.GetBlocksInRectangularRadius(_areaNeedApplierSpec.ApplicationRadius);
	}

	private bool IsInfluencedByApplier(Vector2Int coordinates)
	{
		return _influencedBlocks.Contains(coordinates);
	}

	private void ApplyNeed(Character character, NeedApplierEffectSpec spec)
	{
		NeedManager component = character.GetComponent<NeedManager>();
		if ((bool)(BaseComponent)(object)component && component.HasNeed(spec.NeedId))
		{
			InstantEffect effect = spec.ToInstantEffect();
			component.ApplyEffect(in effect);
			NeedApplied?.Invoke(this, new NeedAppliedEventArgs(character, effect));
		}
	}
}

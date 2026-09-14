using System;
using System.Collections.Frozen;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.GameFactionSystem;
using Timberborn.GameSceneLoading;
using Timberborn.NewGameConfigurationSystem;
using Timberborn.Persistence;
using Timberborn.SceneLoading;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.NeedApplication;

public class EffectProbabilityService : ILoadableSingleton, ISaveableSingleton
{
	private static readonly SingletonKey EffectProbabilityServiceKey = new SingletonKey("EffectProbabilityService");

	private static readonly PropertyKey<float> InjuryChanceModifierKey = new PropertyKey<float>("InjuryChanceModifier");

	private readonly ISceneLoader _sceneLoader;

	private readonly ISingletonLoader _singletonLoader;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly ISpecService _specService;

	private readonly FactionNeedService _factionNeedService;

	private float _injuryChanceModifier;

	private FrozenDictionary<string, ProbabilityGroupSpec> _probabilityGroups;

	public bool CanApplyEffects => _injuryChanceModifier > 0f;

	public EffectProbabilityService(ISceneLoader sceneLoader, ISingletonLoader singletonLoader, IRandomNumberGenerator randomNumberGenerator, ISpecService specService, FactionNeedService factionNeedService)
	{
		_sceneLoader = sceneLoader;
		_singletonLoader = singletonLoader;
		_randomNumberGenerator = randomNumberGenerator;
		_specService = specService;
		_factionNeedService = factionNeedService;
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(EffectProbabilityServiceKey, out var objectLoader))
		{
			_injuryChanceModifier = objectLoader.Get(InjuryChanceModifierKey);
		}
		else
		{
			GameModeSpec gameMode = _sceneLoader.GetSceneParameters<GameSceneParameters>().NewGameConfiguration.GameMode;
			_injuryChanceModifier = gameMode.InjuryChance;
		}
		_probabilityGroups = _specService.GetSingleSpec<ProbabilityGroupsSpec>().Groups.ToFrozenDictionary((ProbabilityGroupSpec group) => group.Id);
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		singletonSaver.GetSingleton(EffectProbabilityServiceKey).Set(InjuryChanceModifierKey, _injuryChanceModifier);
	}

	public bool CanApply(IProbabilityGroupProvider probabilityGroupProvider, NeedApplierEffectSpec spec)
	{
		if (CanApplyEffects)
		{
			float effectProbability = GetEffectProbability(spec, probabilityGroupProvider.ProbabilityGroupId);
			return _randomNumberGenerator.CheckProbability(effectProbability);
		}
		return false;
	}

	private float GetEffectProbability(NeedApplierEffectSpec spec, string probabilityGroupId)
	{
		ProbabilityGroupSpec probabilityGroupSpec = _probabilityGroups[probabilityGroupId];
		float num = spec.Probability switch
		{
			EffectProbability.Low => probabilityGroupSpec.Low, 
			EffectProbability.Medium => probabilityGroupSpec.Medium, 
			EffectProbability.High => probabilityGroupSpec.High, 
			_ => throw new ArgumentOutOfRangeException($"Unknown probability: {spec.Probability}"), 
		};
		if (!_factionNeedService.GetBeaverOrBotNeedById(spec.NeedId).HasSpec<InjurableNeedSpec>())
		{
			return num;
		}
		return num * _injuryChanceModifier;
	}
}

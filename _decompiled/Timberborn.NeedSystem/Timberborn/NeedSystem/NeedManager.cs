using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Bots;
using Timberborn.Characters;
using Timberborn.Effects;
using Timberborn.EntitySystem;
using Timberborn.GameFactionSystem;
using Timberborn.NeedSpecs;
using Timberborn.Persistence;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.NeedSystem;

public class NeedManager : TickableComponent, IAwakableComponent, IPersistentEntity, IChildhoodInfluenced, IRegisteredComponent
{
	private static readonly ComponentKey NeedManagerKey = new ComponentKey("NeedManager");

	private static readonly ListKey<SerializedNeed> NeedsKey = new ListKey<SerializedNeed>("Needs");

	private readonly IDayNightCycle _dayNightCycle;

	private readonly SerializedNeedValueSerializer _serializedNeedValueSerializer;

	private readonly FactionNeedService _factionNeedService;

	private Character _character;

	private Needs _needs;

	private Need _nullNeed;

	public ImmutableArray<NeedSpec> NeedSpecs { get; private set; }

	public event EventHandler<NeedChangedCriticalStateEventArgs> NeedChangedCriticalState;

	public event EventHandler<NeedChangedIsAtMinimumStateEventArgs> NeedChangedIsAtMinimumState;

	public event EventHandler<NeedChangedIsFavorableEventArgs> NeedChangedIsFavorable;

	public event EventHandler<NeedChangedActiveStateEventArgs> NeedChangedActiveState;

	public NeedManager(FactionNeedService factionNeedService, IDayNightCycle dayNightCycle, SerializedNeedValueSerializer serializedNeedValueSerializer)
	{
		_factionNeedService = factionNeedService;
		_dayNightCycle = dayNightCycle;
		_serializedNeedValueSerializer = serializedNeedValueSerializer;
	}

	public void Awake()
	{
		_character = GetComponent<Character>();
		InitializeNeeds();
	}

	public override void Tick()
	{
		UpdateNeeds();
	}

	public void InfluenceByChildhood(Character child)
	{
		foreach (Need allNeed in child.GetComponent<NeedManager>()._needs.AllNeeds)
		{
			if (_needs.TryGetNeed(allNeed.NeedSpec.Id, out var need))
			{
				bool isAtMinimumPoints = need.IsAtMinimumPoints;
				bool isFavorable = need.IsFavorable;
				bool isInCriticalState = need.IsInCriticalState;
				bool isActive = need.IsActive;
				need.SetPoints(allNeed.Points);
				CheckNewState(need, isAtMinimumPoints, isFavorable, isInCriticalState, isActive);
			}
		}
	}

	public bool NeedIsActive(string needId)
	{
		return GetNeed(needId).IsActive;
	}

	public bool NeedIsEnabled(string needId)
	{
		return GetNeed(needId).Enabled;
	}

	public float NeedPointsToMax(string needId)
	{
		return GetNeed(needId).PointsToMax;
	}

	public float GetNeedPoints(string needId)
	{
		return GetNeed(needId).Points;
	}

	public NeedSpec GetNeedSpec(string needId)
	{
		return _needs.GetNeedSpec(needId);
	}

	public bool NeedIsInCriticalState(string needId)
	{
		return GetNeed(needId).IsInCriticalState;
	}

	public bool NeedIsAtMinimumPoints(string needId)
	{
		return GetNeed(needId).IsAtMinimumPoints;
	}

	public bool NeedIsFavorable(string needId)
	{
		return GetNeed(needId).IsFavorable;
	}

	public int GetNeedWellbeing(string needId)
	{
		return GetNeed(needId).Wellbeing;
	}

	public bool NeedIsBelowWarningThreshold(string needId)
	{
		return GetNeed(needId).IsBelowWarningThreshold;
	}

	public bool AnyNeedIsInCriticalState()
	{
		return _needs.Any((Need need) => need.IsInCriticalState);
	}

	public bool NeedIsCritical(string needId)
	{
		return GetNeed(needId).IsCritical;
	}

	public bool HasNeed(string needId)
	{
		return _needs.Has(needId);
	}

	public void EnableUpdate(string needId)
	{
		GetNeed(needId).EnableUpdate();
	}

	public void DisableUpdate(string needId)
	{
		GetNeed(needId).DisableUpdate();
	}

	public void EnableNeed(string needId)
	{
		GetNeed(needId).EnableNeed();
	}

	public void DisableNeed(string needId)
	{
		GetNeed(needId).DisableNeed();
	}

	public void ResetNeed(string needId)
	{
		Need need = GetNeed(needId);
		bool isAtMinimumPoints = need.IsAtMinimumPoints;
		bool isFavorable = need.IsFavorable;
		bool isInCriticalState = need.IsInCriticalState;
		bool isActive = need.IsActive;
		need.Reset();
		CheckNewState(need, isAtMinimumPoints, isFavorable, isInCriticalState, isActive);
	}

	public bool TryAppraise(string needId, in Effect effect, out float points)
	{
		return GetNeed(needId).TryAppraise(effect, out points);
	}

	public void ApplyEffect(in ContinuousEffect effect, float deltaTimeInHours)
	{
		ApplyEffect(ToInstantEffect(in effect, deltaTimeInHours));
	}

	public void ApplyEffect(in InstantEffect effect)
	{
		if (_needs.TryGetNeed(effect.NeedId, out var need))
		{
			bool isAtMinimumPoints = need.IsAtMinimumPoints;
			bool isFavorable = need.IsFavorable;
			bool isInCriticalState = need.IsInCriticalState;
			bool isActive = need.IsActive;
			need.Apply(Effect.From(effect));
			CheckNewState(need, isAtMinimumPoints, isFavorable, isInCriticalState, isActive);
		}
	}

	public void ApplyEffects(IReadOnlyList<ContinuousEffect> effects, float deltaTimeInHours)
	{
		for (int i = 0; i < effects.Count; i++)
		{
			ApplyEffect(effects[i], deltaTimeInHours);
		}
	}

	public float FullyEffectiveDurationInHours(ContinuousEffect effect)
	{
		return GetNeed(effect.NeedId).EffectiveDurationInHours(effect);
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(NeedManagerKey).Set<SerializedNeed>(values: GetSerializedNeeds().ToList(), key: NeedsKey, serializer: _serializedNeedValueSerializer);
	}

	public void Load(IEntityLoader entityLoader)
	{
		foreach (SerializedNeed item in entityLoader.GetComponent(NeedManagerKey).Get(NeedsKey, _serializedNeedValueSerializer))
		{
			Need need2;
			if (_needs.TryGetNeed(item.Id, out var need))
			{
				LoadNeed(need, item.Points);
			}
			else if (_needs.TryGetBackwardCompatibleNeed(item.Id, out need2))
			{
				LoadNeed(need2, item.Points);
			}
			else
			{
				Debug.Log("Beaver doesn't have a " + item.Id + " found in save, ignoring it.");
			}
		}
	}

	private IEnumerable<SerializedNeed> GetSerializedNeeds()
	{
		return from needSpec in NeedSpecs
			select GetNeed(needSpec.Id) into need
			where need.Points != 0f || need.NeedSpec.StartingValue != 0f
			select new SerializedNeed(need.NeedSpec.Id, need.Points);
	}

	private void InitializeNeeds()
	{
		NeedSpecs = GetNeeds().ToImmutableArray();
		float deltaTimeInHours = _dayNightCycle.FixedDeltaTimeInHours;
		IEnumerable<Need> needs = NeedSpecs.Select((NeedSpec spec) => new Need(spec, deltaTimeInHours));
		_needs = new Needs(needs);
		_nullNeed = new Need(_factionNeedService.NullNeed, deltaTimeInHours);
	}

	private IEnumerable<NeedSpec> GetNeeds()
	{
		if (!HasComponent<BotSpec>())
		{
			return _factionNeedService.GetBeaverNeeds();
		}
		return _factionNeedService.GetBotNeeds();
	}

	private Need GetNeed(string id)
	{
		if (!_needs.TryGetNeed(id, out var need))
		{
			return _nullNeed;
		}
		return need;
	}

	private void UpdateNeeds()
	{
		if (_character.Alive)
		{
			ImmutableArray<Need> allNeeds = _needs.AllNeeds;
			for (int i = 0; i < allNeeds.Length; i++)
			{
				UpdateNeed(allNeeds[i]);
			}
		}
	}

	private void UpdateNeed(Need need)
	{
		bool isAtMinimumPoints = need.IsAtMinimumPoints;
		bool isFavorable = need.IsFavorable;
		bool isInCriticalState = need.IsInCriticalState;
		bool isActive = need.IsActive;
		need.Update();
		CheckNewState(need, isAtMinimumPoints, isFavorable, isInCriticalState, isActive);
	}

	private void CheckNewState(Need need, bool wasAtMinimumPoints, bool wasFavorable, bool wasInCriticalState, bool wasActive)
	{
		bool isAtMinimumPoints = need.IsAtMinimumPoints;
		if (wasAtMinimumPoints != isAtMinimumPoints)
		{
			NeedChangedIsAtMinimumState?.Invoke(this, new NeedChangedIsAtMinimumStateEventArgs(need.NeedSpec, isAtMinimumPoints));
		}
		bool isFavorable = need.IsFavorable;
		if (wasFavorable != isFavorable)
		{
			NeedChangedIsFavorable?.Invoke(this, new NeedChangedIsFavorableEventArgs(need.NeedSpec));
		}
		if (need.IsCritical)
		{
			bool isInCriticalState = need.IsInCriticalState;
			if (wasInCriticalState != isInCriticalState)
			{
				NeedChangedCriticalState?.Invoke(this, new NeedChangedCriticalStateEventArgs(need.NeedSpec, isInCriticalState));
			}
		}
		bool isActive = need.IsActive;
		if (wasActive != isActive)
		{
			NeedChangedActiveState?.Invoke(this, new NeedChangedActiveStateEventArgs(need.NeedSpec, isActive));
		}
	}

	private void LoadNeed(Need need, float points)
	{
		need.SetPoints(points);
		NeedSpec needSpec = need.NeedSpec;
		NeedChangedIsAtMinimumState?.Invoke(this, new NeedChangedIsAtMinimumStateEventArgs(needSpec, need.IsAtMinimumPoints));
		if (need.IsCritical)
		{
			NeedChangedCriticalState?.Invoke(this, new NeedChangedCriticalStateEventArgs(needSpec, need.IsInCriticalState));
		}
		NeedChangedIsFavorable?.Invoke(this, new NeedChangedIsFavorableEventArgs(needSpec));
		NeedChangedActiveState?.Invoke(this, new NeedChangedActiveStateEventArgs(needSpec, need.IsActive));
	}

	private static InstantEffect ToInstantEffect(in ContinuousEffect continuousEffect, float deltaTimeInHours)
	{
		float points = continuousEffect.PointsPerHour * deltaTimeInHours;
		return new InstantEffect(continuousEffect.NeedId, points, 1);
	}
}

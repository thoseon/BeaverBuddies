using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Effects;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using UnityEngine;

namespace Timberborn.NeedBehaviorSystem;

public class DistrictNeedBehaviorService : BaseComponent
{
	private class NeedBehaviorGroup
	{
		public ImmutableArray<string> Needs { get; }

		public ImmutableArray<InstantEffect> Effects { get; }

		public List<NeedBehavior> NeedBehaviors { get; } = new List<NeedBehavior>();

		public NeedBehaviorGroup(IEnumerable<string> needs, IEnumerable<InstantEffect> effects)
		{
			Needs = needs.ToImmutableArray();
			Effects = effects.ToImmutableArray();
		}

		public void AddBehavior(NeedBehavior needBehavior)
		{
			NeedBehaviors.Add(needBehavior);
		}

		public void RemoveBehavior(NeedBehavior needBehavior)
		{
			NeedBehaviors.Remove(needBehavior);
		}
	}

	private readonly struct AppraisedNeedBehaviorGroup : IComparable<AppraisedNeedBehaviorGroup>
	{
		public NeedBehaviorGroup NeedBehaviorGroup { get; }

		public float Points { get; }

		public AppraisedNeedBehaviorGroup(NeedBehaviorGroup needBehaviorGroup, float points)
		{
			NeedBehaviorGroup = needBehaviorGroup;
			Points = points;
		}

		public int CompareTo(AppraisedNeedBehaviorGroup other)
		{
			if (!(Points < other.Points))
			{
				return -1;
			}
			return 1;
		}
	}

	private readonly NeedBehaviorKeyGenerator _needBehaviorKeyGenerator;

	private readonly Dictionary<string, NeedBehaviorGroup> _needBehaviors = new Dictionary<string, NeedBehaviorGroup>();

	private readonly SortedSet<AppraisedNeedBehaviorGroup> _appraisedNeedBehaviors = new SortedSet<AppraisedNeedBehaviorGroup>();

	public DistrictNeedBehaviorService(NeedBehaviorKeyGenerator needBehaviorKeyGenerator)
	{
		_needBehaviorKeyGenerator = needBehaviorKeyGenerator;
	}

	public void AddNeedBehavior(IReadOnlyList<InstantEffectSpec> effects, NeedBehavior needBehavior)
	{
		GetNeedBehaviors(effects).AddBehavior(needBehavior);
	}

	public void AddNeedBehavior(IReadOnlyList<ContinuousEffectSpec> effects, NeedBehavior needBehavior)
	{
		GetNeedBehaviors(effects).AddBehavior(needBehavior);
	}

	public void RemoveNeedBehavior(IReadOnlyList<InstantEffectSpec> effects, NeedBehavior needBehavior)
	{
		GetNeedBehaviors(effects).RemoveBehavior(needBehavior);
	}

	public void RemoveNeedBehavior(IReadOnlyList<ContinuousEffectSpec> effects, NeedBehavior needBehavior)
	{
		GetNeedBehaviors(effects).RemoveBehavior(needBehavior);
	}

	public AppraisedAction? PickBestAction(NeedManager needManager, Vector3 essentialActionPosition, float hoursLeftForNonEssentialActions, NeedFilter needFilter)
	{
		AppraiseNeedBehaviors(needManager, needFilter);
		AppraisedAction? result = PickShortestAction(needManager, essentialActionPosition, hoursLeftForNonEssentialActions, needFilter.OnlyCriticalStateNeeds);
		_appraisedNeedBehaviors.Clear();
		return result;
	}

	private void AppraiseNeedBehaviors(NeedManager needManager, NeedFilter needFilter)
	{
		Appraiser component = needManager.GetComponent<Appraiser>();
		foreach (NeedBehaviorGroup value in _needBehaviors.Values)
		{
			float num = component.AppraiseEffects(value.Effects, needFilter);
			if (num > 0f)
			{
				_appraisedNeedBehaviors.Add(new AppraisedNeedBehaviorGroup(value, num));
			}
		}
	}

	private AppraisedAction? PickShortestAction(NeedManager needManager, Vector3 essentialActionPosition, float hoursLeftForNonEssentialActions, bool onlyNeedsInCriticalState)
	{
		ActionDurationCalculator component = needManager.GetComponent<ActionDurationCalculator>();
		foreach (AppraisedNeedBehaviorGroup appraisedNeedBehavior in _appraisedNeedBehaviors)
		{
			AppraisedAction? appraisedAction = null;
			float num = float.MaxValue;
			float points = appraisedNeedBehavior.Points;
			NeedBehaviorGroup needBehaviorGroup = appraisedNeedBehavior.NeedBehaviorGroup;
			List<NeedBehavior> needBehaviors = needBehaviorGroup.NeedBehaviors;
			for (int i = 0; i < needBehaviors.Count; i++)
			{
				NeedBehavior needBehavior = needBehaviors[i];
				Vector3? vector = needBehavior.ActionPosition(needManager);
				if (vector.HasValue)
				{
					Vector3 valueOrDefault = vector.GetValueOrDefault();
					float num2 = component.DurationWithReturnInHours(valueOrDefault, essentialActionPosition);
					if (((hoursLeftForNonEssentialActions > num2) | onlyNeedsInCriticalState) && num2 < num)
					{
						appraisedAction = new AppraisedAction(needBehavior, needBehaviorGroup.Needs, points);
						num = num2;
					}
				}
			}
			if (appraisedAction.HasValue)
			{
				return appraisedAction.Value;
			}
		}
		return null;
	}

	private NeedBehaviorGroup GetNeedBehaviors(IReadOnlyList<InstantEffectSpec> effectSpecs)
	{
		string key = _needBehaviorKeyGenerator.GenerateKey(effectSpecs);
		if (!_needBehaviors.TryGetValue(key, out var value))
		{
			IEnumerable<string> needs = effectSpecs.Select((InstantEffectSpec effect) => effect.NeedId);
			IEnumerable<InstantEffect> effects = effectSpecs.Select((InstantEffectSpec effect) => InstantEffect.FromSpec(effect, 100));
			value = new NeedBehaviorGroup(needs, effects);
			_needBehaviors[key] = value;
		}
		return value;
	}

	private NeedBehaviorGroup GetNeedBehaviors(IReadOnlyList<ContinuousEffectSpec> effectSpecs)
	{
		string key = _needBehaviorKeyGenerator.GenerateKey(effectSpecs);
		if (!_needBehaviors.TryGetValue(key, out var value))
		{
			IEnumerable<string> needs = effectSpecs.Select((ContinuousEffectSpec effect) => effect.NeedId);
			IEnumerable<InstantEffect> effects = effectSpecs.Select((ContinuousEffectSpec effect) => InstantEffect.DiscretizeContinuousEffect(effect.NeedId));
			value = new NeedBehaviorGroup(needs, effects);
			_needBehaviors[key] = value;
		}
		return value;
	}
}

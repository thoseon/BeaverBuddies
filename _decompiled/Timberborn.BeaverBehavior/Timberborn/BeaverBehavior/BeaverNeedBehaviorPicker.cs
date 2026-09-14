using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.GameDistricts;
using Timberborn.NeedBehaviorSystem;
using Timberborn.NeedSystem;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.BeaverBehavior;

internal class BeaverNeedBehaviorPicker : BaseComponent, IAwakableComponent, IPersistentEntity, INeedBehaviorPicker
{
	private static readonly ComponentKey NeedBehaviorPickerKey = new ComponentKey("BeaverNeedBehaviorPicker");

	private static readonly ListKey<string> NeedsBeingCriticallySatisfiedKey = new ListKey<string>("NeedsBeingCriticallySatisfied");

	private readonly IDayNightCycle _dayNightCycle;

	private NeedManager _needManager;

	private Appraiser _appraiser;

	private Citizen _citizen;

	private ActionDurationCalculator _actionDurationCalculator;

	private readonly HashSet<string> _needsBeingCriticallySatisfied = new HashSet<string>();

	private EssentialNeedBehavior _essentialBehavior;

	public BeaverNeedBehaviorPicker(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_needManager = GetComponent<NeedManager>();
		_appraiser = GetComponent<Appraiser>();
		_citizen = GetComponent<Citizen>();
		_actionDurationCalculator = GetComponent<ActionDurationCalculator>();
	}

	public void InitializeEssentialNeedBehavior(EssentialNeedBehavior essentialNeedBehavior)
	{
		if (_essentialBehavior == null)
		{
			_essentialBehavior = essentialNeedBehavior;
			return;
		}
		throw new InvalidOperationException("There can be only one essential behavior " + $"and it's already {_essentialBehavior}");
	}

	public Behavior GetBestNeedBehaviorAffectingNeedsInCriticalState()
	{
		if (!_needManager.AnyNeedIsInCriticalState())
		{
			return null;
		}
		return GetBestNeedBehavior(NeedFilter.NeedIsInCriticalState(_needManager));
	}

	public Behavior GetBestNeedBehavior()
	{
		if (!_needManager.AnyNeedIsInCriticalState())
		{
			return GetBestNeedBehavior(NeedFilter.AnyNeed());
		}
		return GetBestNeedBehavior(NeedFilter.NeedIsCritical(_needManager));
	}

	public bool NeedIsBeingCriticallySatisfied(string needId)
	{
		return _needsBeingCriticallySatisfied.Contains(needId);
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_needsBeingCriticallySatisfied.Count > 0)
		{
			entitySaver.GetComponent(NeedBehaviorPickerKey).Set(NeedsBeingCriticallySatisfiedKey, _needsBeingCriticallySatisfied);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(NeedBehaviorPickerKey, out var objectLoader))
		{
			_needsBeingCriticallySatisfied.UnionWith(objectLoader.Get(NeedsBeingCriticallySatisfiedKey));
		}
	}

	private Behavior GetBestNeedBehavior(NeedFilter needFilter)
	{
		float num = _dayNightCycle.HoursToNextStartOf(TimeOfDay.Daytime);
		EssentialAction essentialAction = _essentialBehavior.GetEssentialAction();
		float num2 = _actionDurationCalculator.FullyEffectiveDurationInHours(in essentialAction);
		float hoursLeftForNonEssentialActions = num - num2;
		AppraisedAction bestNonEssentialAction = GetBestNonEssentialAction(essentialAction.Position, hoursLeftForNonEssentialActions, needFilter);
		_needsBeingCriticallySatisfied.Clear();
		if (ShouldPickEssentialAction(essentialAction, num2, bestNonEssentialAction, num))
		{
			AddCriticallySatisfiedNeed(essentialAction.Effect.NeedId);
			return _essentialBehavior;
		}
		Behavior needBehavior = bestNonEssentialAction.NeedBehavior;
		if (needBehavior != null)
		{
			AddCriticallySatisfiedNeeds(bestNonEssentialAction.AffectedNeeds);
			return needBehavior;
		}
		return null;
	}

	private bool ShouldPickEssentialAction(EssentialAction essentialAction, float fullDurationOfEssentialActionInHours, AppraisedAction nonEssentialAction, float hoursToDawn)
	{
		float essentialActionPoints = _appraiser.AppraiseEffect(in essentialAction);
		if (!ItIsTimeForEssentialAction(essentialActionPoints, fullDurationOfEssentialActionInHours, nonEssentialAction, hoursToDawn))
		{
			return EssentialActionIsAtMinimumPoints(essentialActionPoints, essentialAction.Effect.NeedId, nonEssentialAction);
		}
		return true;
	}

	private static bool ItIsTimeForEssentialAction(float essentialActionPoints, float fullDurationOfEssentialActionInHours, AppraisedAction nonEssentialAction, float hoursToDawn)
	{
		if (nonEssentialAction.NeedBehavior == null && essentialActionPoints > 0f)
		{
			return hoursToDawn < fullDurationOfEssentialActionInHours * 1.2f;
		}
		return false;
	}

	private bool EssentialActionIsAtMinimumPoints(float essentialActionPoints, string essentialActionNeedId, AppraisedAction nonEssentialAction)
	{
		if (essentialActionPoints > nonEssentialAction.Points)
		{
			return _needManager.NeedIsAtMinimumPoints(essentialActionNeedId);
		}
		return false;
	}

	private AppraisedAction GetBestNonEssentialAction(Vector3 essentialActionPosition, float hoursLeftForNonEssentialActions, NeedFilter needFilter)
	{
		if (_citizen.HasAssignedDistrict)
		{
			AppraisedAction? appraisedAction = _citizen.AssignedDistrict.GetComponent<DistrictNeedBehaviorService>().PickBestAction(_needManager, essentialActionPosition, hoursLeftForNonEssentialActions, needFilter);
			if (appraisedAction.HasValue)
			{
				return appraisedAction.GetValueOrDefault();
			}
		}
		return default(AppraisedAction);
	}

	private void AddCriticallySatisfiedNeeds(ImmutableArray<string> needIds)
	{
		for (int i = 0; i < needIds.Length; i++)
		{
			AddCriticallySatisfiedNeed(needIds[i]);
		}
	}

	private void AddCriticallySatisfiedNeed(string needId)
	{
		if (_needManager.NeedIsInCriticalState(needId))
		{
			_needsBeingCriticallySatisfied.Add(needId);
		}
	}
}

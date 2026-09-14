using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.GameDistricts;
using Timberborn.NeedBehaviorSystem;
using Timberborn.NeedSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.BotBehavior;

internal class BotNeedBehaviorPicker : BaseComponent, IAwakableComponent, IPersistentEntity, INeedBehaviorPicker
{
	private static readonly ComponentKey BotNeedBehaviorPickerKey = new ComponentKey("BotNeedBehaviorPicker");

	private static readonly ListKey<string> NeedsBeingCriticallySatisfiedKey = new ListKey<string>("NeedsBeingCriticallySatisfied");

	private NeedManager _needManager;

	private Citizen _citizen;

	private readonly HashSet<string> _needsBeingCriticallySatisfied = new HashSet<string>();

	public void Awake()
	{
		_needManager = GetComponent<NeedManager>();
		_citizen = GetComponent<Citizen>();
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
			return GetBestNeedBehavior(NeedFilter.NeedIsBelowWarningThreshold(_needManager));
		}
		return GetBestNeedBehavior(NeedFilter.NeedIsInCriticalState(_needManager));
	}

	public bool NeedIsBeingCriticallySatisfied(string needId)
	{
		return _needsBeingCriticallySatisfied.Contains(needId);
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_needsBeingCriticallySatisfied.Count > 0)
		{
			entitySaver.GetComponent(BotNeedBehaviorPickerKey).Set(NeedsBeingCriticallySatisfiedKey, _needsBeingCriticallySatisfied);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(BotNeedBehaviorPickerKey, out var objectLoader))
		{
			_needsBeingCriticallySatisfied.UnionWith(objectLoader.Get(NeedsBeingCriticallySatisfiedKey));
		}
	}

	private Behavior GetBestNeedBehavior(NeedFilter needFilter)
	{
		AppraisedAction bestNonEssentialAction = GetBestNonEssentialAction(base.Transform.position, needFilter);
		_needsBeingCriticallySatisfied.Clear();
		Behavior needBehavior = bestNonEssentialAction.NeedBehavior;
		if (needBehavior != null)
		{
			AddCriticallySatisfiedNeeds(bestNonEssentialAction.AffectedNeeds);
			return needBehavior;
		}
		return null;
	}

	private AppraisedAction GetBestNonEssentialAction(Vector3 essentialActionPosition, NeedFilter needFilter)
	{
		if (_citizen.HasAssignedDistrict)
		{
			AppraisedAction? appraisedAction = _citizen.AssignedDistrict.GetComponent<DistrictNeedBehaviorService>().PickBestAction(_needManager, essentialActionPosition, float.MaxValue, needFilter);
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
		if (_needManager.NeedIsBelowWarningThreshold(needId))
		{
			_needsBeingCriticallySatisfied.Add(needId);
		}
	}
}

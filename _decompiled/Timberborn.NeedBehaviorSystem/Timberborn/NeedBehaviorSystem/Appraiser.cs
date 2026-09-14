using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.Effects;
using Timberborn.NeedSystem;

namespace Timberborn.NeedBehaviorSystem;

public class Appraiser : BaseComponent, IAwakableComponent
{
	private NeedManager _needManager;

	public void Awake()
	{
		_needManager = GetComponent<NeedManager>();
	}

	public float AppraiseEffects(IEnumerable<InstantEffect> instantEffects)
	{
		float num = 0f;
		foreach (InstantEffect instantEffect in instantEffects)
		{
			if (_needManager.TryAppraise(instantEffect.NeedId, Effect.From(instantEffect), out var points))
			{
				num += points;
				continue;
			}
			return 0f;
		}
		return num;
	}

	public float AppraiseEffect(in EssentialAction essentialAction)
	{
		string needId = essentialAction.Effect.NeedId;
		InstantEffect instantEffect = InstantEffect.DiscretizeContinuousEffect(needId);
		if (!_needManager.TryAppraise(needId, Effect.From(instantEffect), out var points))
		{
			return 0f;
		}
		return points;
	}

	public float AppraiseEffects(ImmutableArray<InstantEffect> effects, NeedFilter needFilter)
	{
		float num = 0f;
		bool flag = false;
		for (int i = 0; i < effects.Length; i++)
		{
			InstantEffect instantEffect = effects[i];
			string needId = instantEffect.NeedId;
			if (_needManager.TryAppraise(needId, Effect.From(instantEffect), out var points))
			{
				if (needFilter.Filter(needId) && points > 0f)
				{
					flag = true;
				}
				num += points;
				continue;
			}
			return 0f;
		}
		if (!flag)
		{
			return 0f;
		}
		return num;
	}
}

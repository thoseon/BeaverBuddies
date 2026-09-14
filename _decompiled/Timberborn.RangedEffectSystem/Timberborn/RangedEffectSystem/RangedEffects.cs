using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.Effects;

namespace Timberborn.RangedEffectSystem;

internal class RangedEffects
{
	private readonly List<RangedEffect> _effects = new List<RangedEffect>();

	private readonly List<RangedEffect> _activeEffects = new List<RangedEffect>();

	public ReadOnlyList<RangedEffect> ActiveEffects => _activeEffects.AsReadOnlyList();

	public IEnumerable<RangedEffectApplier> RangedEffectAppliers => _effects.SelectMany((RangedEffect rangedEffect) => rangedEffect.Appliers);

	public void Add(RangedEffectApplier rangedEffectApplier)
	{
		foreach (ContinuousEffect effect in rangedEffectApplier.Effects)
		{
			RangedEffect orCreate = GetOrCreate(effect);
			orCreate.Add(rangedEffectApplier);
			if (rangedEffectApplier.Active)
			{
				AddActiveEffect(orCreate);
			}
		}
		rangedEffectApplier.ActiveChanged += OnActiveChanged;
	}

	public void Remove(RangedEffectApplier rangedEffectApplier)
	{
		rangedEffectApplier.ActiveChanged -= OnActiveChanged;
		foreach (ContinuousEffect effect in rangedEffectApplier.Effects)
		{
			if (TryGetRangedEffect(effect, out var result))
			{
				result.Remove(rangedEffectApplier);
				if (!result.Appliers.Any())
				{
					_effects.Remove(result);
					_activeEffects.Remove(result);
				}
				else
				{
					RemoveInactiveEffect(result);
				}
			}
		}
	}

	private void OnActiveChanged(object sender, ActiveChangedEventArgs e)
	{
		RangedEffectApplier obj = (RangedEffectApplier)sender;
		bool state = e.State;
		foreach (ContinuousEffect effect in obj.Effects)
		{
			if (TryGetRangedEffect(effect, out var result))
			{
				if (state)
				{
					AddActiveEffect(result);
				}
				else
				{
					RemoveInactiveEffect(result);
				}
			}
		}
	}

	private void AddActiveEffect(RangedEffect effect)
	{
		if (!_activeEffects.Contains(effect))
		{
			_activeEffects.Add(effect);
		}
	}

	private void RemoveInactiveEffect(RangedEffect effect)
	{
		if (!effect.IsActive)
		{
			_activeEffects.Remove(effect);
		}
	}

	private RangedEffect GetOrCreate(ContinuousEffect effect)
	{
		if (TryGetRangedEffect(effect, out var result))
		{
			return result;
		}
		RangedEffect rangedEffect = new RangedEffect(effect);
		_effects.Add(rangedEffect);
		return rangedEffect;
	}

	private bool TryGetRangedEffect(ContinuousEffect effect, out RangedEffect result)
	{
		foreach (RangedEffect effect2 in _effects)
		{
			if (effect2.BaseEffect == effect)
			{
				result = effect2;
				return true;
			}
		}
		result = null;
		return false;
	}
}

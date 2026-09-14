using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.AchievementSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.TemplateSystem;
using Timberborn.WaterSourceSystem;

namespace Timberborn.Achievements;

internal abstract class PlugBadwaterSourceAchievement : Achievement
{
	private static readonly string BadwaterSourceTemplate = "BadwaterSource";

	private readonly EntityComponentRegistry _entityComponentRegistry;

	private readonly List<WaterSource> _waterSources = new List<WaterSource>();

	private readonly bool _mustPlugAll;

	public override string Id { get; }

	protected PlugBadwaterSourceAchievement(EntityComponentRegistry entityComponentRegistry, bool mustPlugAll, string id)
	{
		_entityComponentRegistry = entityComponentRegistry;
		_mustPlugAll = mustPlugAll;
		Id = id;
	}

	protected override void EnableInternal()
	{
		_waterSources.AddRange(from source in _entityComponentRegistry.GetEnabled<WaterSource>()
			where source.GetComponent<TemplateSpec>().TemplateName == BadwaterSourceTemplate
			select source);
		if (IsPlugConditionValidated())
		{
			Unlock();
			return;
		}
		foreach (WaterSource waterSource in _waterSources)
		{
			waterSource.WaterStrengthModifierAdded += OnWaterStrengthModifierAdded;
		}
	}

	protected override void DisableInternal()
	{
		foreach (WaterSource waterSource in _waterSources)
		{
			waterSource.WaterStrengthModifierAdded -= OnWaterStrengthModifierAdded;
		}
	}

	private void OnWaterStrengthModifierAdded(object sender, EventArgs eventArgs)
	{
		if (IsPlugConditionValidated())
		{
			Unlock();
		}
	}

	private bool IsPlugConditionValidated()
	{
		if (_waterSources.Count > 0)
		{
			if (!_mustPlugAll || !_waterSources.FastAll(IsPlugged))
			{
				if (!_mustPlugAll)
				{
					return _waterSources.FastAny(IsPlugged);
				}
				return false;
			}
			return true;
		}
		return false;
	}

	private static bool IsPlugged(WaterSource waterSource)
	{
		foreach (IWaterStrengthModifier waterStrengthModifier in waterSource.WaterStrengthModifiers)
		{
			if ((waterStrengthModifier is WaterSourceDisabler || waterStrengthModifier is WaterSourceRegulator { IsOpen: false }) ? true : false)
			{
				return true;
			}
		}
		return false;
	}
}

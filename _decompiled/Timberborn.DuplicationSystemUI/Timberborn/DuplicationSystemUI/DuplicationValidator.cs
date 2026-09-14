using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.ToolSystem;

namespace Timberborn.DuplicationSystemUI;

internal class DuplicationValidator
{
	private readonly IEnumerable<IToolFinder> _toolFinders;

	public DuplicationValidator(IEnumerable<IToolFinder> toolFinders)
	{
		_toolFinders = toolFinders;
	}

	public bool CanDuplicateSettings(BaseComponent entity)
	{
		return entity.AllComponents.FastAny((object component) => component is IDuplicable duplicable && duplicable.IsDuplicable);
	}

	public bool CanDuplicateObject(BaseComponent entity, out Action toolActivationAction)
	{
		toolActivationAction = null;
		if (!entity.HasComponent<DuplicationBlocker>())
		{
			foreach (IToolFinder toolFinder in _toolFinders)
			{
				if (toolFinder.TryFindTool(entity, out toolActivationAction))
				{
					return true;
				}
			}
		}
		return false;
	}
}

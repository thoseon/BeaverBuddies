using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.GameWonderCompletion;
using Timberborn.Localization;

namespace Timberborn.WondersUI;

internal class WonderDescriber : BaseComponent, IEntityDescriber
{
	private static readonly string WonderLocKey = "Buildings.Wonder";

	private static readonly string WonderDescriptionLocKey = "Buildings.Wonder.Description";

	private static readonly string WonderDescriptionCompletedLocKey = "Buildings.Wonder.DescriptionCompleted";

	private readonly ILoc _loc;

	private readonly GameWonderCompletionService _wonderCompletionService;

	public WonderDescriber(ILoc loc, GameWonderCompletionService wonderCompletionService)
	{
		_loc = loc;
		_wonderCompletionService = wonderCompletionService;
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		string key = (_wonderCompletionService.IsWonderCompletedWithAnyFaction() ? WonderDescriptionCompletedLocKey : WonderDescriptionLocKey);
		string content = SpecialStrings.RowStarter + _loc.T(WonderLocKey) + "\n" + SpecialStrings.RowStarter + _loc.T(key);
		yield return EntityDescription.CreateTextSection(content, 2040);
	}
}

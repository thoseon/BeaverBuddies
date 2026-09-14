using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Localization;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class BuildingTutorialStep : ITutorialStep, ITutorialStepWithTool
{
	private readonly BuiltBuildingService _builtBuildingService;

	private readonly ILoc _loc;

	private readonly ImmutableArray<string> _templateNames;

	private readonly bool _onlyFinishedBuildings;

	private readonly int _requiredAmount;

	private readonly string _mainLocKey;

	private readonly string _localizedBuildingName;

	public bool KeepBlinking => NumberOfAllBuildings < _requiredAmount;

	private int NumberOfBuildings
	{
		get
		{
			if (!_onlyFinishedBuildings)
			{
				return NumberOfAllBuildings;
			}
			return _builtBuildingService.NumberOfFinishedBuildings(_templateNames);
		}
	}

	private int NumberOfAllBuildings => _builtBuildingService.NumberOfAllBuildings(_templateNames);

	public BuildingTutorialStep(BuiltBuildingService builtBuildingService, ILoc loc, IEnumerable<string> templateNames, bool onlyFinishedBuildings, int requiredAmount, string mainLocKey, string localizedBuildingName)
	{
		_builtBuildingService = builtBuildingService;
		_loc = loc;
		_templateNames = templateNames.ToImmutableArray();
		_onlyFinishedBuildings = onlyFinishedBuildings;
		_requiredAmount = requiredAmount;
		_mainLocKey = mainLocKey;
		_localizedBuildingName = localizedBuildingName;
	}

	public string Description()
	{
		return _loc.T(_mainLocKey, _localizedBuildingName, Math.Min(NumberOfBuildings, _requiredAmount), _requiredAmount);
	}

	public bool Achieved()
	{
		return NumberOfBuildings >= _requiredAmount;
	}
}

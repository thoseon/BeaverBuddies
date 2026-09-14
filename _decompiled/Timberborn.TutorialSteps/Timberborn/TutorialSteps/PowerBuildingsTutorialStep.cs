using System;
using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.Localization;
using Timberborn.MechanicalSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class PowerBuildingsTutorialStep : ITutorialStep
{
	private static readonly string PowerBuildingLocKey = "Tutorial.PowerBuilding";

	private readonly BuiltBuildingService _builtBuildingService;

	private readonly ILoc _loc;

	private readonly string _templateName;

	private readonly int _requiredAmount;

	private readonly string _localizedBuildingName;

	private int NumberOfPoweredBuildings => _builtBuildingService.GetFinishedBuildings(_templateName).FastCount(delegate(Building building)
	{
		MechanicalGraph graph = building.GetComponent<MechanicalNode>().Graph;
		return graph != null && graph.NumberOfGenerators > 0;
	});

	public PowerBuildingsTutorialStep(BuiltBuildingService builtBuildingService, ILoc loc, string templateName, int requiredAmount, string localizedBuildingName)
	{
		_builtBuildingService = builtBuildingService;
		_loc = loc;
		_templateName = templateName;
		_requiredAmount = requiredAmount;
		_localizedBuildingName = localizedBuildingName;
	}

	public string Description()
	{
		return _loc.T(PowerBuildingLocKey, _localizedBuildingName, Math.Min(NumberOfPoweredBuildings, _requiredAmount), _requiredAmount);
	}

	public bool Achieved()
	{
		return NumberOfPoweredBuildings >= _requiredAmount;
	}
}

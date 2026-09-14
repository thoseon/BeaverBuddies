using System;
using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.GameDistricts;
using Timberborn.Localization;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class ConnectBuildingsTutorialStep : ITutorialStep
{
	private static readonly string ConnectBuildingLocKey = "Tutorial.ConnectBuilding";

	private readonly BuiltBuildingService _builtBuildingService;

	private readonly ILoc _loc;

	private readonly string _templateName;

	private readonly int _requiredAmount;

	private readonly string _localizedBuildingName;

	private readonly bool _countUnfinishedBuildings;

	private int NumberOfConnectedBuildings => NumberOfConnectedFinishedBuildings + (_countUnfinishedBuildings ? NumberOfConnectedUnfinishedBuildings : 0);

	private int NumberOfConnectedFinishedBuildings => _builtBuildingService.GetFinishedBuildings(_templateName).FastCount((Building building) => building.GetComponent<DistrictBuilding>().InstantDistrict);

	private int NumberOfConnectedUnfinishedBuildings => _builtBuildingService.GetUnfinishedBuildings(_templateName).FastCount((Building building) => building.GetComponent<DistrictBuilding>().ConstructionDistrict);

	public ConnectBuildingsTutorialStep(BuiltBuildingService builtBuildingService, ILoc loc, string templateName, int requiredAmount, string localizedBuildingName, bool countUnfinishedBuildings)
	{
		_builtBuildingService = builtBuildingService;
		_loc = loc;
		_templateName = templateName;
		_requiredAmount = requiredAmount;
		_localizedBuildingName = localizedBuildingName;
		_countUnfinishedBuildings = countUnfinishedBuildings;
	}

	public string Description()
	{
		return _loc.T(ConnectBuildingLocKey, _localizedBuildingName, Math.Min(NumberOfConnectedBuildings, _requiredAmount), _requiredAmount);
	}

	public bool Achieved()
	{
		return NumberOfConnectedBuildings >= _requiredAmount;
	}
}

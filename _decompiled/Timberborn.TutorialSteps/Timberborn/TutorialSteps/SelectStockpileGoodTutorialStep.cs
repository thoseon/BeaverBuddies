using System;
using System.Collections.Generic;
using Timberborn.Buildings;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.Localization;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class SelectStockpileGoodTutorialStep : ITutorialStep
{
	private static readonly string ProgressLocKey = "Tutorial.Progress";

	private readonly BuiltBuildingService _builtBuildingService;

	private readonly ILoc _loc;

	private readonly string _templateName;

	private readonly GoodSpec _requiredGood;

	private readonly int _requiredAmount;

	private readonly string _mainLocKey;

	private readonly string _localizedBuildingName;

	public SelectStockpileGoodTutorialStep(BuiltBuildingService builtBuildingService, ILoc loc, string templateName, GoodSpec requiredGood, int requiredAmount, string mainLocKey, string localizedBuildingName)
	{
		_builtBuildingService = builtBuildingService;
		_loc = loc;
		_templateName = templateName;
		_requiredGood = requiredGood;
		_requiredAmount = requiredAmount;
		_mainLocKey = mainLocKey;
		_localizedBuildingName = localizedBuildingName;
	}

	public string Description()
	{
		int param = Math.Min(GetNumberOfValidBuildings(), _requiredAmount);
		string param2 = _loc.T(ProgressLocKey, param, _requiredAmount);
		string value = _requiredGood.PluralDisplayName.Value;
		return _loc.T(_mainLocKey, _localizedBuildingName, value, param2);
	}

	public bool Achieved()
	{
		return GetNumberOfValidBuildings() >= _requiredAmount;
	}

	private int GetNumberOfValidBuildings()
	{
		IReadOnlyList<Building> finishedBuildings = _builtBuildingService.GetFinishedBuildings(_templateName);
		IReadOnlyList<Building> unfinishedBuildings = _builtBuildingService.GetUnfinishedBuildings(_templateName);
		int numberOfValidBuildings = GetNumberOfValidBuildings(finishedBuildings);
		if (numberOfValidBuildings < _requiredAmount)
		{
			return numberOfValidBuildings + GetNumberOfValidBuildings(unfinishedBuildings);
		}
		return numberOfValidBuildings;
	}

	private int GetNumberOfValidBuildings(IReadOnlyList<Building> buildingsSpecs)
	{
		int num = 0;
		for (int i = 0; i < buildingsSpecs.Count; i++)
		{
			if (buildingsSpecs[i].GetComponent<SingleGoodAllower>().AllowedGood == _requiredGood.Id && ++num >= _requiredAmount)
			{
				return num;
			}
		}
		return num;
	}
}

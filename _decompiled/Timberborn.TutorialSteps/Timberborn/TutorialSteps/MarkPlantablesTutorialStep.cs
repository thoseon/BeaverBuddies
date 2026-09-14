using System;
using Timberborn.Localization;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class MarkPlantablesTutorialStep : ITutorialStep
{
	private static readonly string MarkPlantablesLocKey = "Tutorial.MarkPlantables";

	private readonly PlantableResourceCounter _plantableResourceCounter;

	private readonly ILoc _loc;

	private readonly string _templateName;

	private readonly int _requiredAmount;

	private readonly string _localizedResourceName;

	private int NumberOfResources => _plantableResourceCounter.GetNumberOfResources(_templateName);

	public MarkPlantablesTutorialStep(PlantableResourceCounter plantableResourceCounter, ILoc loc, string templateName, int requiredAmount, string localizedResourceName)
	{
		_plantableResourceCounter = plantableResourceCounter;
		_loc = loc;
		_templateName = templateName;
		_requiredAmount = requiredAmount;
		_localizedResourceName = localizedResourceName;
	}

	public string Description()
	{
		return _loc.T(MarkPlantablesLocKey, _localizedResourceName, Math.Min(NumberOfResources, _requiredAmount), _requiredAmount);
	}

	public bool Achieved()
	{
		return NumberOfResources >= _requiredAmount;
	}
}

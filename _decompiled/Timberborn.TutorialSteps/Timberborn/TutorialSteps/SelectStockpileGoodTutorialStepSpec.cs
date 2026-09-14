using Timberborn.BlueprintSystem;

namespace Timberborn.TutorialSteps;

internal record SelectStockpileGoodTutorialStepSpec : ComponentSpec
{
	[Serialize]
	public string TemplateName { get; init; }

	[Serialize]
	public int RequiredAmount { get; init; }

	[Serialize]
	public string GoodId { get; init; }
}

using Timberborn.FeatureToggleSystem;

namespace Timberborn.BlueprintSystem;

internal record DisablingFeatureToggleSpec : ComponentSpec
{
	[Serialize]
	public string Toggle { get; init; }

	public bool Disabled => FeatureToggleService.IsToggleOn(Toggle);
}

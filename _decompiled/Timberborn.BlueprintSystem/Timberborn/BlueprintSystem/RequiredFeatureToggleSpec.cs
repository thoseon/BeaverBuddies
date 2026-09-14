using Timberborn.FeatureToggleSystem;

namespace Timberborn.BlueprintSystem;

internal record RequiredFeatureToggleSpec : ComponentSpec
{
	[Serialize]
	public string Toggle { get; init; }

	public bool Disabled => !FeatureToggleService.IsToggleOn(Toggle);
}

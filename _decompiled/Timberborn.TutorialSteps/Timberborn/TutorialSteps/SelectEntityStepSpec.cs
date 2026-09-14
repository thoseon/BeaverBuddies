using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.LocalizationSerialization;

namespace Timberborn.TutorialSteps;

internal record SelectEntityStepSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> TemplateNames { get; init; }

	[Serialize("DescriptionLocKey")]
	public LocalizedText Description { get; init; }

	[Serialize]
	private string DescriptionLocKey { get; init; }
}

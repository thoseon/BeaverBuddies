using Timberborn.BlueprintSystem;
using Timberborn.LocalizationSerialization;

namespace Timberborn.WorkSystem;

public record NeedPreventingWorkSpec : ComponentSpec
{
	[Serialize("WorkRefusalWarningLocKey")]
	public LocalizedText WorkRefusalWarning { get; init; }

	[Serialize]
	private string WorkRefusalWarningLocKey { get; init; }
}

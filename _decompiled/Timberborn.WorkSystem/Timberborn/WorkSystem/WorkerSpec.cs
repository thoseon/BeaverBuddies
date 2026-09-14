using Timberborn.BlueprintSystem;

namespace Timberborn.WorkSystem;

internal record WorkerSpec : ComponentSpec
{
	[Serialize]
	public string WorkerType { get; init; }
}

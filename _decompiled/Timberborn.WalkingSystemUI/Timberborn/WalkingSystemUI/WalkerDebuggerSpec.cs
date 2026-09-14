using Timberborn.BlueprintSystem;

namespace Timberborn.WalkingSystemUI;

internal record WalkerDebuggerSpec : ComponentSpec
{
	[Serialize]
	public string WalkerGameObjectMarkerPath { get; init; }

	[Serialize]
	public string WalkerModelMarkerPath { get; init; }

	[Serialize]
	public string DestinationMarkerPath { get; init; }

	[Serialize]
	public string CornerMarkerPath { get; init; }
}

using Timberborn.BlueprintSystem;

namespace Timberborn.MechanicalConnectorSystem;

internal record MechanicalConnectorFactorySpec : ComponentSpec
{
	[Serialize]
	public string MechanicalConnectorPrefabPath { get; init; }
}

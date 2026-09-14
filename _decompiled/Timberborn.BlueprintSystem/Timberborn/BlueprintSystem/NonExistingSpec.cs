namespace Timberborn.BlueprintSystem;

public record NonExistingSpec : ComponentSpec
{
	[Serialize]
	public string SpecName { get; init; }

	[Serialize]
	public string Content { get; init; }
}
